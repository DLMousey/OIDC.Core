using System;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class UserRoleTest
{
    [Fact]
    public void NewInstanceHasNoGuids()
    {
        UserRole userRole = new UserRole();
        
        Assert.True(userRole.UserId.Equals(Guid.Empty));
        Assert.True(userRole.RoleId.Equals(Guid.Empty));
    }
}