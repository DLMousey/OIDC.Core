using System;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class ScopeTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        Scope scope = new Scope();
        Assert.False(scope.Id.Equals(Guid.Empty));
    }

    [Fact]
    public void NewInstanceIsMarkedDangerous()
    {
        Scope scope = new Scope();
        Assert.True(scope.Dangerous);
    }
}