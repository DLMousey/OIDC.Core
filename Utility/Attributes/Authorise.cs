using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Authorise : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            User user = (User) context.HttpContext.Items["User"];
            if (user == null || user.Banned)
            {
                string message = (user == null)
                    ? "Unauthorised"
                    : $"Unauthorised: Your account was terminated by the service on {user.BannedAt.ToString()}"; 
                
                context.Result = new JsonResult(new
                {
                    status = 401,
                    message
                }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}