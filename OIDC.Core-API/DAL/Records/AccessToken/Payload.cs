using System;
using Newtonsoft.Json;

namespace OAuthServer.DAL.Records.AccessToken;

public record Payload(
    [property: JsonProperty("iss")] string Issuer, 
    [property: JsonProperty("sub")] Guid Subject, 
    [property: JsonProperty("aud")] string Audience
)
{
    [JsonProperty("nbf")] 
    public DateTime NotBeforeTime { get; } = DateTime.UtcNow;

    [JsonProperty("iat")]
    public DateTime IssuedAtTime { get; } = DateTime.UtcNow;

    [JsonProperty("jti")]
    public Guid JWTId { get; } = Guid.NewGuid();
}