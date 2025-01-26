
using System.Text.Json.Serialization;

namespace Qonq.BlueSky.Model
{
    public class AspectRatio
    {
        [JsonPropertyName("width")]
        public int width { get; set; }

        [JsonPropertyName("height")]
        public int height { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("alt")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string alt { get; set; }

        [JsonPropertyName("image")]
        public Blob image { get; set; }

        [JsonPropertyName("aspectRatio")]
        public AspectRatio aspectRatio { get; set; }
    }

    public class Images
    {
        [JsonPropertyName("images")]
        public List<Image> imageslist { get; set; }
    }
}
