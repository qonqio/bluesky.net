using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model
{
	public class CreateSessionRequest
	{
		[JsonPropertyName("identifier")]
		public string Identifier { get; set; }
		[JsonPropertyName("password")]
		public string Password { get; set; }
	}
}