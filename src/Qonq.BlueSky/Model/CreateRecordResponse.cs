using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model;

public class CreateRecordResponse
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; }
    [JsonPropertyName("cid")]
    public string Cid { get; set; }
}