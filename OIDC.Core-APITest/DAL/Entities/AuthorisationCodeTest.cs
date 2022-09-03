using System;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class AuthorisationCodeTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        AuthorisationCode code = new AuthorisationCode();
        Assert.False(code.Id.Equals(Guid.Empty));
    }
}