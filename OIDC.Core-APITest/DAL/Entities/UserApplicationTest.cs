using System;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class UserApplicationTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        UserApplication application = new UserApplication();
        Assert.False(application.Id.Equals(Guid.Empty));
    }
}