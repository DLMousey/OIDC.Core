using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.Records.AccessToken;
using OAuthServer.Services.Interface;
using Org.BouncyCastle.Asn1.X509;

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
        byte[] signingKey = System.Text.Encoding.UTF8.GetBytes(_configuration.GetValue<string>("jwt.signingKey"));
        string type = _configuration.GetValue<string>("jwt.type");
        string algorithm = _configuration.GetValue<string>("jwt.algorithm");
        string issuer = _configuration.GetValue<string>("jwt.issuer");
        
        Header header = new Header(type, algorithm);
        Payload payload = new Payload(issuer, accessToken.UserId, accessToken.Application.HomepageUrl);

        string headerJson = JsonConvert.SerializeObject(header);
        string payloadJson = JsonConvert.SerializeObject(payload);

        string headerB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(headerJson));
        string payloadB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payloadJson));

        HMACSHA256 hash = new HMACSHA256(signingKey);
        string signature = Convert.ToBase64String(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(headerB64 + "." + payloadB64)));
        
        return new AccessTokenJwt(header, payload, signature);
    }

    public bool ValidateJwt(AccessTokenJwt jwt)
    {
        throw new NotImplementedException();
    }

    public AccessTokenJwt DecodeJwt(string accessToken)
    {
        string[] parts = accessToken.Split('.');

        if (parts.Length != 3)
        {
            throw new ArgumentException("Invalid access token structure");
        }

        Header header = JsonConvert.DeserializeObject<Header>(parts[0]);
        Payload payload = JsonConvert.DeserializeObject<Payload>(parts[1]);
        string signature = parts[2];

        return new AccessTokenJwt(header, payload, signature);
    }

    public string EncodeJwt(AccessTokenJwt jwt)
    {
        string tokenOut = string.Empty;

        string headerJson = JsonConvert.SerializeObject(jwt.Header);
        string payloadJson = JsonConvert.SerializeObject(jwt.Payload);

        byte[] headerBytes = System.Text.Encoding.UTF8.GetBytes(headerJson);
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payloadJson);

        tokenOut += Convert.ToBase64String(headerBytes) + ".";
        tokenOut += Convert.ToBase64String(payloadBytes) + ".";

        return tokenOut;
    }
}