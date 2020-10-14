using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualBasic;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Authorise : Attribute, IAuthorizationFilter
    {
        private readonly string[] _requiredScopes;

        public Authorise(string requiredScopes = null)
        {
            if (requiredScopes == null)
            {
                _requiredScopes = null;
                return;
            }
            
            string[] scopeNames = requiredScopes.Split(",");
            for (int i = 0; i < scopeNames.Length; i++)
            {
                scopeNames[i] = scopeNames[i].Trim();
            }
            
            _requiredScopes = requiredScopes.Split(", ");
        }
        
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

            /*
             * If no scopes have been provided to the attribute - no scopes are required
             * to access this endpoint, just a valid access token which has already been checked
             * by the middleware.
             */
            if (_requiredScopes == null)
            {
                return;
            }

            /*
             * Pull the list of available scope instances out of the HttpContext (which were added by the
             * middleware), if there are no available scopes but some were defined - abort. 
             */
            List<Scope> availableScopes = (List<Scope>) context.HttpContext.Items["Scopes"];
            if (_requiredScopes.Length > 0 && availableScopes == null)
            {
                string message = "Required scopes not granted to access this endpoint";

                context.Result = new JsonResult(new
                {
                    status = 403,
                    message
                }) { StatusCode = StatusCodes.Status403Forbidden };
            }

            /*
             * Build up a new list of provided scopes by matching the name of available scopes
             * to required scopes passed as arguments to the attribute
             */
            List<Scope> providedScopes = new List<Scope>();
            foreach (string scopeName in _requiredScopes)
            {
                Scope requiredScope = availableScopes
                    .FirstOrDefault(s => s.Name.Equals(scopeName));
                
                if (requiredScope != null)
                {
                    providedScopes.Add(requiredScope);
                }
            }

            /*
             * Only scopes that match the arguments passed to the attribute by name will be added
             * to the providedScopes list, if the lengths do not match - abort.
             */
            if (providedScopes.Count != _requiredScopes.Length)
            {
                string message = "Required scopes not granted to access this endpoint";

                context.Result = new JsonResult(new
                {
                    status = 403,
                    message
                }) { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }
}