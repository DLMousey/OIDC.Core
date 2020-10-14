using System;
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

        public VerifyAccessToken(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context, IAccessTokenService accessTokenService)
        {
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
                await AttachUserToContext(context, accessCode.Replace("Bearer ", ""), accessTokenService);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string accessCode, IAccessTokenService accessTokenService)
        {
            AccessToken accessToken = await accessTokenService.FindByCodeAsync(accessCode);
            if (accessToken.Revoked || DateTime.Now > accessToken.ExpiresAt)
            {
                return;
            }

            context.Items["User"] = accessToken.User;
            context.Items["Application"] = accessToken.Application;
        }
    }
}