using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Utility.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthoriseScopes : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredScopes;

    public AuthoriseScopes(string requiredScopes = null)
    {
        if (requiredScopes == null)
        {
            _requiredScopes = null;
            return;
        }

        string[] scopeNames = requiredScopes.Split(",");
        for (int i = 0; i < scopeNames.Length; i++)
        {
            scopeNames[i] = scopeNames[i].Trim().ToLower();
        }

        _requiredScopes = requiredScopes.Split(", ");
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        /*
         * If no scopes have been provided to the attributes - no scopes are required
         * to access this endpoint, just a valid access token which has already been checked
         * by the middleware.
         */
        if (_requiredScopes == null)
        {
            return;
        }
        
        List<Scope> availableScopes = (List<Scope>)context.HttpContext.Items["Scopes"];
        string[] availableScopeNames = availableScopes.Select(s => s.Name.ToLower()).ToArray();
        
        bool isAuthorised = true;

        foreach (string scopeName in _requiredScopes)
        {
            if (!availableScopeNames.Contains(scopeName.ToLower()))
            {
                isAuthorised = false;
            }
        }

        if (!isAuthorised)
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