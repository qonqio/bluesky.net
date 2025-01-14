using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model
{
	public class BlueSkyUser
	{
		[JsonPropertyName("did")]
		public string Did { get; set; }
		[JsonPropertyName("handle")]
		public string Handle { get; set; }
		[JsonPropertyName("displayName")]
		public string DisplayName { get; set; }
		[JsonPropertyName("avatar")]
		public string Avatar { get; set; }
		[JsonPropertyName("viewer")]
		public Viewer Viewer { get; set; }
		[JsonPropertyName("labels")]
		public List<object> Labels { get; set; }
		[JsonPropertyName("createdAt")]
		public DateTime CreatedAt { get; set; }
		[JsonPropertyName("description")]
		public string Description { get; set; }
		[JsonPropertyName("indexedAt")]
		public DateTime IndexedAt { get; set; }

	}
}