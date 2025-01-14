using System.Text.Json.Serialization;

namespace Qonq.BlueSky.Model
{
    public class Record
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        [JsonPropertyName("cid")]
        public string Cid { get; set; }
        [JsonPropertyName("value")]
        public RecordValue Value { get; set; }
    }
}
