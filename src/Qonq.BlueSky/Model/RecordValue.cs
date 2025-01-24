using Qonq.BlueSky.Helper;
using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model
{
	public class RecordValue
	{
		[JsonPropertyName("text")]
		public string Text { get; set; }

		[JsonPropertyName("subject")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Subject { get; set; }

		[JsonPropertyName("createdAt")]
		public string CreatedAt { get; set; }


		[JsonPropertyName("$type")]
		public string Type { get; set; }

        [JsonPropertyName("facets")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Facet>? Facets { get; set; }

        [JsonPropertyName("embed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Embed? Embed { get; set; }
    }
}