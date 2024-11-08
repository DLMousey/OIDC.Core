using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OAuthServer.DAL.Entities;
using OAuthServer.Services.Interface;

namespace OAuthServer.Middleware
{
    public class VerifyAccessToken
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        private readonly IList<string> _excludedPaths;

        public VerifyAccessToken(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;

            _excludedPaths = config.GetSection("Middleware:VerifyAccessToken:ExcludedPaths").Get<List<string>>();
        }

        public async Task Invoke(HttpContext context, IAccessTokenService accessTokenService, 
            IUserApplicationService userApplicationService)
        {
            string path = context.Request.Path.Value;
            if (_excludedPaths.Contains(path))
            {
                await _next(context);
                return;
            }
            
            
            string accessCode;
            bool hasCookie = context.Request.Cookies.TryGetValue("_oidc.core-token", out accessCode);

            if (!hasCookie)
            {
                accessCode = context.Request.Headers
                    .FirstOrDefault(x => x.Key.Equals("Authorization"))
                    .Value;
            }

            if (accessCode != null)
            {
                await AttachUserToContext(
                    context, 
                    accessCode.Replace("Bearer ", ""), 
                    accessTokenService,
                    userApplicationService
                );
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string accessCode, 
            IAccessTokenService accessTokenService, IUserApplicationService userApplicationService)
        {
            AccessToken accessToken = await accessTokenService.FindByCodeAsync(accessCode);
            if (accessToken.Revoked || DateTime.Now > accessToken.ExpiresAt)
            {
                return;
            }

            context.Items["User"] = accessToken.User;
            context.Items["Application"] = accessToken.Application;

            if (accessToken.Application != null)
            {
                List<Scope> scopes = new List<Scope>();
                List<Role> roles = new List<Role>();
                UserApplication userApplication =
                    await userApplicationService.FindByUserAndApplicationAsync(accessToken.User,
                        accessToken.Application);

                if (userApplication == null)
                {
                    return;
                }

                foreach (UserApplicationScope scope in userApplication.Scopes)
                {
                    scopes.Add(scope.Scope);
                }

                foreach (Role role in userApplication.User.Roles)
                {
                    roles.Add(role);
                }

                context.Items["Scopes"] = scopes;
                context.Items["Roles"] = roles;
            }
        }
    }
}