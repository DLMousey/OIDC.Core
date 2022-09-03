using System;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class AccessTokenTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        AccessToken token = new AccessToken();
        Assert.False(token.Id.Equals(Guid.Empty));
    }
}