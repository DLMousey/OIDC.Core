using System;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class UserTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        User user = new User();
        Assert.False(user.Id.Equals(Guid.Empty));
    }
}