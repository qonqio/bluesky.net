using System.Text.Json.Serialization;

namespace Qonq.BlueSky.Model
{
    public class Facet
    {
        public string Type { get; set; }

        [JsonPropertyName("index")]
        public Index Index { get; set; }

        [JsonPropertyName("features")]
        public List<Feature> Features { get; set; }
    }

    public class Feature
    {
        [JsonPropertyName("$type")]
        public string Type { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("did")]
        public string Did { get; set; }
    }

    public class Index
    {

        [JsonPropertyName("byteStart")]
        public int ByteStart { get; set; }

        [JsonPropertyName("byteEnd")]
        public int ByteEnd { get; set; }
    }


}
