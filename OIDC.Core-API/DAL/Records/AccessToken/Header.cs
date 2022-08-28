using Newtonsoft.Json;

namespace OAuthServer.DAL.Records.AccessToken;

public record Header(
    [property: JsonProperty("alg")] string Algorithm, 
    [property: JsonProperty("typ")] string Type
);