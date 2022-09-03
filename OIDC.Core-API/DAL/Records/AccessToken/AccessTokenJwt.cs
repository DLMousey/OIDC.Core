using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace OAuthServer.DAL.Records.AccessToken;

public record AccessTokenJwt(Header Header, Payload Payload, string Signature = null)
{
    [property: JsonProperty("signature")] public string Signature { get; set; }

    public override string ToString()
    {
        string headerB64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Header.ToString()));
        string payloadB64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Payload.ToString()));

        return $"{headerB64}.{payloadB64}.{Signature}";
    }
}