namespace OAuthServer.DAL.Records.AccessToken;

public record AccessTokenJwt(Header Header, Payload Payload, string Signature);