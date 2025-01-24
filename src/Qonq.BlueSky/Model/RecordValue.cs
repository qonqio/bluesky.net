using Qonq.BlueSky.Helper;
using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model
{
	public class RecordValue
	{
		[JsonPropertyName("text")]
		public string Text { get; set; }

		[JsonPropertyName("subject")]
		public string Subject { get; set; }

		[JsonPropertyName("createdAt")]
		public string CreatedAt { get; set; }


		[JsonPropertyName("$type")]
		public string Type { get; set; }

        [JsonPropertyName("facets")]
        public List<Facet>? Facets { get; set; }
    }
}