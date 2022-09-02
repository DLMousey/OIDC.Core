using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OAuthServer.DAL.Records.AccessToken;
using OAuthServer.Services.Implementation;
using OAuthServer.Services.Interface;
using Xunit;

namespace OIDC.Core_APITest.Services;

public class JwtServiceTest
{
    private readonly IJwtService _jwtService;

    public JwtServiceTest()
    {
        Dictionary<string, string> mockConfiguration = new Dictionary<string, string>
        {
            { "jwt.type", "JWT" },
            { "jwt.algorithm", "HSA256" },
            { "jwt.issuer", "https://oidc.core" },
            { "jwt.signingKey", "testSigningKey" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(mockConfiguration)
            .Build();

        _jwtService = new JwtService(configuration);
    }

    [Fact]
    public void CanEncodeHeader()
    {
        Header header = new Header("HS256", "JWT");
        string expected = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
        string actual = _jwtService.EncodeTokenPart(header);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanEncodePayload()
    {
        Guid guid = Guid.Parse("b3fe1620-18ad-4a51-be6d-5d89f3f3e455");

        Payload payload = new Payload("https://oidc.core", guid, "https://example.com");

        string json = JsonConvert.SerializeObject(payload);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        string payloadB64 = Convert.ToBase64String(jsonBytes);

        string actual = _jwtService.EncodeTokenPart(payload);

        Assert.Equal(payloadB64, actual);
    }

    [Fact]
    public void CanCreateObjectFromString()
    {
        Guid guid = Guid.Parse("b3fe1620-18ad-4a51-be6d-5d89f3f3e455");
        string token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiM2ZlMTYyMC0xOGFkLTRhNTEtYmU2ZC01ZDg5ZjNmM2U0NTUiLCJpc3MiOiJodHRwczovL29pZGMuY29yZSIsImF1ZCI6Imh0dHBzOi8vZXhhbXBsZS5jb20iLCJpYXQiOjE1MTYyMzkwMjJ9.Shq8KoSScUpflpXf26lmzoSq9-MatDl9tv7REtgXlag";

        AccessTokenJwt jwt = _jwtService.CreateFromString(token);
        DateTime iat = new DateTime(2018, 01, 18, 01, 30, 22);

        Assert.Equal("HS256", jwt.Header.Algorithm);
        Assert.Equal("JWT", jwt.Header.Type);
        Assert.Equal("https://oidc.core", jwt.Payload.Issuer);
        Assert.Equal("https://example.com", jwt.Payload.Audience);
        Assert.Equal(iat, jwt.Payload.IssuedAtTime);
        Assert.Equal(guid, jwt.Payload.Subject);
        Assert.Equal("Shq8KoSScUpflpXf26lmzoSq9-MatDl9tv7REtgXlag", jwt.Signature);
    }

    [Fact]
    public void CanCreateStringFromObject()
    {
        Guid guid = Guid.Parse("b3fe1620-18ad-4a51-be6d-5d89f3f3e455");
        string rawToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiM2ZlMTYyMC0xOGFkLTRhNTEtYmU2ZC01ZDg5ZjNmM2U0NTUiLCJpc3MiOiJodHRwczovL29pZGMuY29yZSIsImF1ZCI6Imh0dHBzOi8vZXhhbXBsZS5jb20iLCJpYXQiOjE1MTYyMzkwMjJ9.Shq8KoSScUpflpXf26lmzoSq9-MatDl9tv7REtgXlag";

        DateTime iat = new DateTime(2018, 01, 18, 01, 30, 22);
        Header header = new Header("HS256", "JWT");
        Payload payload = new Payload("https://oidc.core", guid, "https://example.com");
        payload.IssuedAtTime = iat;

        AccessTokenJwt accessToken = _jwtService.CreateFromString(rawToken);

        Assert.Equal("HS256", accessToken.Header.Algorithm);
        Assert.Equal("JWT", accessToken.Header.Type);
        Assert.Equal(guid, accessToken.Payload.Subject);
        Assert.Equal("https://oidc.core", accessToken.Payload.Issuer);
        Assert.Equal("https://example.com", accessToken.Payload.Audience);
        Assert.Equal(iat, accessToken.Payload.IssuedAtTime);
        Assert.Equal("Shq8KoSScUpflpXf26lmzoSq9-MatDl9tv7REtgXlag", accessToken.Signature);
    }
}