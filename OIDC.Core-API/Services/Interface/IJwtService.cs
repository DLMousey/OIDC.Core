using OAuthServer.DAL.Entities;
using OAuthServer.DAL.Records.AccessToken;

namespace OAuthServer.Services.Interface;

public interface IJwtService
{
    public AccessTokenJwt CreateJwt(AccessToken accessToken);

    public bool ValidateJwt(AccessTokenJwt jwt);
    
    public AccessTokenJwt DecodeJwt(string jwt);

    public string EncodeJwt(AccessTokenJwt jwt);
}