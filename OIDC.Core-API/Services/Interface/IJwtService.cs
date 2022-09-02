using OAuthServer.DAL.Entities;
using OAuthServer.DAL.Records.AccessToken;

namespace OAuthServer.Services.Interface;

public interface IJwtService
{
    public AccessTokenJwt CreateJwt(AccessToken accessToken);

    public AccessTokenJwt CreateFromString(string accessToken);

    public string CreateFromObject(AccessTokenJwt accessToken);

    public bool ValidateJwt(AccessTokenJwt accessToken);

    public bool ValidateJwt(string accessToken);

    public string EncodeTokenPart(object tokenPart);

    // public bool ValidateJwt(AccessTokenJwt jwt);
    //
    // public AccessTokenJwt DecodeJwt(string jwt);
    //
    // public string EncodeJwt(AccessTokenJwt jwt);
}