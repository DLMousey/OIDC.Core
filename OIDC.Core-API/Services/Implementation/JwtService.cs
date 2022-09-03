using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.Records.AccessToken;
using OAuthServer.Services.Interface;

namespace OAuthServer.Services.Implementation;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public AccessTokenJwt CreateJwt(AccessToken accessToken)
    {
        string type = _configuration.GetValue<string>("jwt.type");
        string algorithm = _configuration.GetValue<string>("jwt.algorithm");
        string issuer = _configuration.GetValue<string>("jwt.issuer");
        
        Header header = new Header(type, algorithm);
        Payload payload = new Payload(issuer, accessToken.UserId, accessToken.Application.HomepageUrl);

        return SignToken(new AccessTokenJwt(header, payload, null));
    }

    public bool ValidateJwt(AccessTokenJwt accessToken)
    {
        return SignToken(accessToken).Signature == accessToken.Signature;
    }

    public bool ValidateJwt(string accessToken)
    {
        AccessTokenJwt token = CreateFromString(accessToken);

        if (token == null)
        {
            throw new ArgumentException("Invalid access token structure");
        }
        
        return SignToken(token).Signature == token.Signature;
    }

    public string CreateFromObject(AccessTokenJwt accessToken)
    {
        string tokenOut = string.Empty;

        string headerJson = JsonConvert.SerializeObject(accessToken.Header);
        string payloadJson = JsonConvert.SerializeObject(accessToken.Payload);

        byte[] headerBytes = Encoding.UTF8.GetBytes(headerJson);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadJson);

        tokenOut += WebEncoders.Base64UrlEncode(headerBytes) + ".";
        tokenOut += WebEncoders.Base64UrlEncode(payloadBytes) + ".";
        tokenOut += accessToken.Signature;

        return tokenOut;
    }

    public AccessTokenJwt CreateFromString(string accessToken)
    {
        string[] parts = accessToken.Split('.');

        if (parts.Length != 3)
        {
            throw new ArgumentException("Invalid access token structure");
        }

        string headerJson = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(parts[0]));
        string payloadJson = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(parts[1]));
        
        Header header = JsonConvert.DeserializeObject<Header>(headerJson);
        Payload payload = JsonConvert.DeserializeObject<Payload>(payloadJson);
        string signature = parts[2];

        return new AccessTokenJwt(header, payload, signature);
    }

    public string EncodeTokenPart(object tokenPart)
    {
        string json = JsonConvert.SerializeObject(tokenPart);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        return WebEncoders.Base64UrlEncode(jsonBytes);
    }

    public AccessTokenJwt SignToken(AccessTokenJwt accessToken)
    {
        byte[] signingKey = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("jwt.signingKey"));
        HMACSHA256 sha256 = new HMACSHA256(signingKey);

        string header = EncodeTokenPart(accessToken.Header);
        string payload = EncodeTokenPart(accessToken.Payload);
        string parts = $"{header}.{payload}";
        
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts));
        string signature = Convert.ToHexString(hash);

        return new AccessTokenJwt(accessToken.Header, accessToken.Payload, signature);
    }
}