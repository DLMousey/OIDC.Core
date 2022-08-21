using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Utility.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthoriseRoles : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredRoles;

    public AuthoriseRoles(string requiredRoles = null)
    {
        if (requiredRoles == null)
        {
            _requiredRoles = null;
            return;
        }

        string[] roleNames = requiredRoles.Split(",");
        for (int i = 0; i < roleNames.Length; i++)
        {
            roleNames[i] = roleNames[i].Trim().ToLower();
        }

        _requiredRoles = requiredRoles.Split(", ");
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        User user = (User)context.HttpContext.Items["User"];
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

            return;
        }
        
        /*
         * If no roles have been provided to the attribute - no roles are required
         * to access this endpoint, just a valid access token which has already been checked
         * by the middleware.
         */
        if (_requiredRoles == null)
        {
            return;
        }
        
        /*
         * Pull the list of available role instances out of the HttpContext (which were added by the
         * middleware), if there are no available roles but some were defined - abort.
         */
        List<Role> availableRoles = (List<Role>)context.HttpContext.Items["Roles"];
        if (_requiredRoles.Length > 0 && availableRoles == null)
        {
            string message = "Required roles not granted to access this endpoint";

            context.Result = new JsonResult(new
            {
                status = 403,
                message
            }) { StatusCode = StatusCodes.Status403Forbidden };

            return;
        }
        
        /*
         * Normalise the names of the roles available from the context
         */
        List<string> normalisedAvailableRoles = new();
        if (availableRoles != null)
            availableRoles.ForEach(r => { normalisedAvailableRoles.Add(r.Name.Trim().ToLower()); });

        /*
         * Build up a new list of provided roles by matching the name of available roles
         * to required roles passed as arguments to the attribute
         */
        List<string> providedRoles = new List<string>();
        foreach (string roleName in _requiredRoles)
        {
            try
            {
                providedRoles.Add(normalisedAvailableRoles.First(rn => rn.Equals(roleName.Trim().ToLower())));
            }
            catch (Exception)
            {
                string message = "Required roles not granted to access this endpoint";

                context.Result = new JsonResult(new
                {
                    status = 403,
                    message
                }) { StatusCode = StatusCodes.Status403Forbidden };

                return;
            }
        }
        
        /*
         * Only (normalised names of) roles that match the arguments passed to the attribute by name
         * will be added to the providedRoles list, if the lengths do not match - abort.
         */
        if (providedRoles.Count != _requiredRoles.Length)
        {
            string message = "Required roles not granted to access this endpoint";

            context.Result = new JsonResult(new
            {
                status = 403,
                message
            }) { StatusCode = StatusCodes.Status403Forbidden };
        }
    }
}