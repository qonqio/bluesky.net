
using System.Text.Json.Serialization;

namespace Qonq.BlueSky.Model
{
    public class Embed
    {
        [JsonPropertyName("$type")]
        public string Type { get; set; }

        [JsonPropertyName("images")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Image>? Images { get; set; }

        [JsonPropertyName("external")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public External? External { get; set; }

    }
}
