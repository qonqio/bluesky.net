using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model
{
	public class DidResponse
	{
		[JsonPropertyName("did")]
		public string Did { get; set; }
	}
}
