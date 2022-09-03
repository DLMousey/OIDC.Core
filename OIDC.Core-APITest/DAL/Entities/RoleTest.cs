using System;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class RoleTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        Role role = new Role();
        Assert.False(role.Id.Equals(Guid.Empty));
    }
}