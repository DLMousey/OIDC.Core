using System.Collections.Generic;
using System.Linq;
using OAuthServer.Services.Implementation;
using OAuthServer.Services.Interface;
using Xunit;

namespace OIDC.Core_APITest.Services;

public class RandomValueServiceTest
{
    private readonly IRandomValueService _randomValueService;

    public RandomValueServiceTest()
    {
        _randomValueService = new RandomValueService();
    }

    [Theory]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    [InlineData(256)]
    [InlineData(512)]
    public void CanGenerateStringsWithSpecificLengths(int length)
    {
        string randomValue = _randomValueService.CryptoSafeRandomString(length);
        
        Assert.Equal(length, randomValue.Length);
    }
}