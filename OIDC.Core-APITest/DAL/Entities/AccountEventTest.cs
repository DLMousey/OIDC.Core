using System;
using OAuthServer.DAL.Entities;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class AccountEventTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        AccountEvent accountEvent = new AccountEvent();
        Assert.False(accountEvent.Guid.Equals(Guid.Empty));
    }
}