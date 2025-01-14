using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model
{
	public class Viewer
	{
		[JsonPropertyName("muted")]
		public bool Muted { get; set; }

		[JsonPropertyName("blockedBy")]
		public bool BlockedBy { get; set; }

		[JsonPropertyName("followedBy")]
		public string FollowedBy { get; set; }
	}
}