using System.Text.Json.Serialization;
namespace Qonq.BlueSky.Model
{
	public class DidResponse
	{
		/// <summary>
		/// The DID of the user.
		/// </summary>
		[JsonPropertyName("did")]
		public string Did { get; set; }
	}
}
