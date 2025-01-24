
using System.Text.Json.Serialization;

namespace Qonq.BlueSky.Model
{
    public class Embed
    {
        [JsonPropertyName("$type")]
        public string Type { get; set; }

        [JsonPropertyName("images")]
        public List<Image> Images { get; set; }

    }
}
