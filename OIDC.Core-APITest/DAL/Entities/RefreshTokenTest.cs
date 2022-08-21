using System;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class RefreshTokenTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        RefreshToken token = new RefreshToken();
        Assert.False(token.Id.Equals(Guid.Empty));
    }
}