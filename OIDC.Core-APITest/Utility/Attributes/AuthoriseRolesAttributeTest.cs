using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using OAuthServer.DAL.Entities;
using OAuthServer.Utility.Attributes;
using Xunit;

namespace OIDC.Core_APITest.Utility.Attributes;

public class AuthoriseRolesAttributeTest
{
    private AuthoriseRoles? _attribute;

    [Fact]
    public async Task HasCorrectUsageAttributes()
    {
        IList <AttributeUsageAttribute> usageAttributes =
            (IList<AttributeUsageAttribute>)typeof(AuthoriseRoles).GetCustomAttributes(typeof(AttributeUsageAttribute),
                false);
        
        Assert.Equal("Class, Method", usageAttributes[0].ValidOn.ToString());
    }

    [Fact]
    public async Task Returns401IfNoUserProvided()
    {
        _attribute = new AuthoriseRoles("admin");

        User user = null;

        Dictionary<object, object?> contextItems = new() { { "User", user } };
        ActionContext actionContext = BuildActionContext(contextItems);
        AuthorizationFilterContext
            context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        
        _attribute.OnAuthorization(context);
        JsonResult result = context.Result as JsonResult ?? throw new InvalidOperationException();

        Assert.IsType<JsonResult>(context.Result);
        Assert.Equal(401, result.StatusCode);

        Debug.Assert(result.Value != null, "result.Value != null");
        Assert.Equal(401, GetJsonResultStatus(result));
        Assert.Equal("Unauthorised", GetJsonResultMessage(result));
    }
    
    [Fact]
    public async Task Returns401IfBannedProvided()
    {
        _attribute = new AuthoriseRoles("admin");

        User user = new User
        {
            Banned = true,
            BannedAt = DateTime.UtcNow
        };

        Dictionary<object, object?> contextItems = new() { { "User", user } };
        ActionContext actionContext = BuildActionContext(contextItems);
        AuthorizationFilterContext
            context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        
        _attribute.OnAuthorization(context);
        JsonResult result = context.Result as JsonResult ?? throw new InvalidOperationException();

        Assert.IsType<JsonResult>(context.Result);
        Assert.Equal(401, result.StatusCode);
        
        string banDt = user.BannedAt.ToString()!;
        string banMessage = $@"Unauthorised: Your account was terminated by the service on {banDt}";

        Debug.Assert(result.Value != null, "result.Value != null");
        Assert.Equal(401, GetJsonResultStatus(result));
        Assert.Equal(banMessage, GetJsonResultMessage(result));
    }

    [Fact]
    public async Task Returns403IfInsufficientRolesAvailable()
    {
        _attribute = new AuthoriseRoles("testAdmin");

        Guid userGuid = new Guid("18943fa3-8c00-4afb-ab68-f59bc90a454f");
        Guid roleGuid = new Guid("dad69341-ce12-4553-bb58-8d51129f02e7");
        Guid role2Guid = new Guid("d22b367d-d9b0-4865-8bab-1c786115ca8d");
        
        Role role = new Role
        {
            Name = "user",
            Id = roleGuid
        };

        User user = new User
        {
            Id = userGuid,
            Banned = false
        };

        IList<Role> roles = new List<Role>();
        roles.Add(role);

        Dictionary<object, object?> contextItems = new() { { "Roles", roles }, { "User", user } };
        ActionContext actionContext = BuildActionContext(contextItems);
        AuthorizationFilterContext context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        
        _attribute.OnAuthorization(context);
        JsonResult result = context.Result as JsonResult ?? throw new InvalidOperationException();

        Assert.IsType<JsonResult>(context.Result);
        Assert.Equal(403, result.StatusCode);
        
        Debug.Assert(result.Value != null, "result.Value != null");
        Assert.Equal(403, GetJsonResultStatus(result));
        Assert.Equal("Required roles not granted to access this endpoint", GetJsonResultMessage(result));
    }
    
    private ActionContext BuildActionContext(IDictionary<object, object?>? contextItems = null)
    {
        ActionContext context = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor()
        };

        if (contextItems == null)
        {
            return context;
        }
        
        foreach (var contextItemsKey in contextItems.Keys)
        {
            context.HttpContext.Items.Add(contextItemsKey, contextItems[contextItemsKey]);
        }

        return context;
    }
    
    private object? GetJsonResultMessage(JsonResult result) =>
        result.Value.GetType().GetProperty("message")!.GetValue(result.Value, null);

    private object? GetJsonResultStatus(JsonResult result) =>
        result.Value.GetType().GetProperty("status")!.GetValue(result.Value, null);
}