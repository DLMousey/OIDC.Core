using OAuthServer.DAL.Entities;
using OAuthServer.DAL.Records.AccessToken;

namespace OAuthServer.Services.Interface;

public interface IJwtService
{
    public string CreateJwt(AccessToken accessToken);

    public bool ValidateJwt(AccessToken accessToken);
}