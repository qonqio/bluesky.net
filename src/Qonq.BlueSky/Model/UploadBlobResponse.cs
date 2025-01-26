using System.Text.Json.Serialization;

namespace Qonq.BlueSky.Model
{


    public class Blob
    {

        [JsonPropertyName("$type")]
        public string type { get; set; }

        [JsonPropertyName("ref")]
        public Ref @ref { get; set; }

        [JsonPropertyName("mimeType")]
        public string mimeType { get; set; }

        [JsonPropertyName("size")]
        public int size { get; set; }
    }

    public class Ref
    {
        
        [JsonPropertyName("$link")]
       
        public string link { get; set; }
    }
    public class UploadBlobResponse
    {
        [JsonPropertyName("blob")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Blob blob { get; set; }
    }
}
