using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OAuthServer.DAL.Records.AccessToken;

public record Payload(
    [property: JsonProperty("iss")] string Issuer, 
    [property: JsonProperty("sub")] Guid Subject, 
    [property: JsonProperty("aud")] string Audience
)
{
    [JsonProperty("nbf")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime NotBeforeTime { get; set; } = DateTime.Now;
    
    [JsonProperty("iat")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime IssuedAtTime { get; set; } = DateTime.Now;

    [JsonProperty("jti")]
    public Guid JWTId { get; set; } = Guid.NewGuid();
}