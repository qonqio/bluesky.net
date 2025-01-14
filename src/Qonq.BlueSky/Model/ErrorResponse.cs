using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model
{
	public class ErrorResponse
	{
		[JsonPropertyName("error")]
		public string Error { get; set; }
		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}
