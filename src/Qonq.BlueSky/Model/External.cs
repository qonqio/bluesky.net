using System.Text.Json.Serialization;

namespace Qonq.BlueSky.Model
{
    public class External
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Description { get; set; }

        [JsonPropertyName("thumb")]
        public Thumb Thumb { get; set; }
    }

    public class Thumb
    {
        [JsonPropertyName("$type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string type { get; set; }

        [JsonPropertyName("@ref")]
        public Ref @ref { get; set; }

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }


        [JsonPropertyName("size")]
        public int Size { get; set; }
    }
}
