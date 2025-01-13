using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model;

public class CreateSessionResponse
{
    [JsonPropertyName("accessJwt")]
    public string AccessJwt { get; set; }
    [JsonPropertyName("refreshJwt")]
    public string RefreshJwt { get; set; }
    [JsonPropertyName("did")]
    public string Did { get; set; }
    [JsonPropertyName("handle")]
    public string Handle { get; set; }
}