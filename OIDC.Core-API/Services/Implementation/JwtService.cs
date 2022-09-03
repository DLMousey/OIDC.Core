using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.Records.AccessToken;
using OAuthServer.Services.Interface;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace OAuthServer.Services.Implementation;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string CreateJwt(AccessToken accessToken)
    {
        string issuer = _configuration.GetValue<string>("Jwt:Issuer");
        string signingKey = _configuration.GetValue<string>("Jwt:SigningKey");
        
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey));

        DateTime now = DateTime.UtcNow;
        DateTime expiry = DateTime.UtcNow.AddMinutes(60);

        IEnumerable<string> roleNames = accessToken.User.Roles.Select(r => r.Role.Name);
        string roleString = String.Join(',', roleNames);

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            IssuedAt = now,
            Audience = accessToken.Application.HomepageUrl,
            Expires = expiry,
            NotBefore = now,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, accessToken.UserId.ToString()),
                new Claim("roles", roleString),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            })
        };

        SecurityToken token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }

    public bool ValidateJwt(AccessToken accessToken)
    {
        string issuer = _configuration.GetValue<string>("Jwt:Issuer");
        string signingKey = _configuration.GetValue<string>("Jwt:SigningKey");

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey));

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(accessToken.Code, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = issuer,
                ValidAudience = accessToken.Application.HomepageUrl,
                IssuerSigningKey = securityKey
            }, out SecurityToken validatedToken);
        }
        catch
        {
            return false;
        }

        return true;
    }
}