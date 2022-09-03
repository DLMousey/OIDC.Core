using System;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class UserApplicationScopeTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        UserApplicationScope uas = new UserApplicationScope();
        
        Assert.True(uas.UserApplicationId.Equals(Guid.Empty));
    }
}