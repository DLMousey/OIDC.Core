using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using OAuthServer.DAL.Entities;
using OAuthServer.Utility.Attributes;
using Xunit;

namespace OIDC.Core_APITest.Utility.Attributes;

public class AuthoriseAttributeTest
{
    private Authorise? _attribute;

    [Fact]
    public async Task Returns401IfNoUserProvided()
    {
        _attribute = new Authorise("profile.read, profile.write");

        ActionContext actionContext = BuildActionContext();
        AuthorizationFilterContext context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        
        _attribute.OnAuthorization(context);
        JsonResult result = context.Result as JsonResult ?? throw new InvalidOperationException();
        
        Assert.IsType<JsonResult>(context.Result);
        Assert.Equal(401, result.StatusCode);
        
        Debug.Assert(result.Value != null, "result.Value != null");
        Assert.Equal(401, GetJsonResultStatus(result));
        Assert.Equal("Unauthorised", GetJsonResultMessage(result));
    }

    [Fact]
    public async Task Returns401IfBannedUserProvided()
    {
        _attribute = new Authorise("profile.read, profile.write");

        User user = new User
        {
            Banned = true,
            BannedAt = DateTime.UtcNow
        };

        Dictionary<object, object?> contextItems = new() { { "User", user } };
        ActionContext actionContext = BuildActionContext(contextItems);
        AuthorizationFilterContext context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

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
    public async Task Returns403IfInsufficientScopesAvailable()
    {
        _attribute = new Authorise("profile.read, profile.write");

        User user = new User
        {
            Banned = false
        };
        Scope scope = new Scope
            { Name = "profile.read", Label = "Read Profile", Description = null, Dangerous = false };
        IList<Scope> scopes = new List<Scope>();
        scopes.Add(scope);

        Dictionary<object, object?> contextItems = new() { { "Scopes", scopes }, { "User", user } };
        ActionContext actionContext = BuildActionContext(contextItems);
        AuthorizationFilterContext context =
            new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        
        _attribute.OnAuthorization(context);
        JsonResult result = context.Result as JsonResult ?? throw new InvalidOperationException();

        Assert.IsType<JsonResult>(context.Result);
        Assert.Equal(403, result.StatusCode);
        
        Debug.Assert(result.Value != null, "result.Value != null");
        Assert.Equal(403, GetJsonResultStatus(result));
        Assert.Equal("Required scopes not granted to access this endpoint", GetJsonResultMessage(result));
    }

    [Fact]
    public async Task Returns403IfNoScopesAvailable()
    {
        _attribute = new Authorise("profile.read, profile.write");

        User user = new User
        {
            Banned = false
        };
        Scope scope = new Scope
            { Name = "profile.read", Label = "Read Profile", Description = null, Dangerous = false };
        IList<Scope> scopes = null;

        Dictionary<object, object?> contextItems = new() { { "Scopes", scopes }, { "User", user } };
        ActionContext actionContext = BuildActionContext(contextItems);
        AuthorizationFilterContext context =
            new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        
        _attribute.OnAuthorization(context);
        JsonResult result = context.Result as JsonResult ?? throw new InvalidOperationException();

        Assert.IsType<JsonResult>(context.Result);
        Assert.Equal(403, result.StatusCode);
        
        Debug.Assert(result.Value != null, "result.Value != null");
        Assert.Equal(403, GetJsonResultStatus(result));
        Assert.Equal("Required scopes not granted to access this endpoint", GetJsonResultMessage(result));
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