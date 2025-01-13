using Qonq.BlueSky.Model;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
namespace Qonq.BlueSky;

public class BlueSkyClient(string pdsHost)
{
	private string _blueSkyHandle;
	private string _accessJwt;
	private string _did;

	/// <summary>
	/// Follow a BlueSky User
	/// </summary>
	/// <param name="userDid">The ID of the user you want to follow</param>
	/// <returns>CreateRecordResponse</returns>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<CreateRecordResponse> FollowUserAsync(string userDid)
	{
		if (_accessJwt == null)
			throw new InvalidOperationException("Must have a valid JWT token");
		if (_blueSkyHandle == null)
			throw new InvalidOperationException("Must have a valid Handle");

		string url = $"{pdsHost}/xrpc/com.atproto.repo.createRecord";

		var createRecordRequest = new CreateRecordRequest
		{
			Repo = _did,
			Collection = "app.bsky.graph.follow",
			Record = new Record
			{
				Subject = userDid,
				CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
			}
		};

		string jsonPayload = JsonSerializer.Serialize(createRecordRequest, new JsonSerializerOptions
		{
			WriteIndented = true // Optional, for pretty printing
		});

		using (var httpClient = new HttpClient())
		{
			httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessJwt}");
			var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

			HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content);

			if (httpResponse.IsSuccessStatusCode)
			{
				var responseContent = await httpResponse.Content.ReadAsStringAsync();
				if (string.IsNullOrWhiteSpace(responseContent))
				{
					throw new InvalidOperationException("Response content is null or empty.");
				}
				var result = JsonSerializer.Deserialize<CreateRecordResponse>(responseContent);
				return result ?? throw new InvalidOperationException("Deserialization returned null. Invalid response data.");
			}
			else
			{
				var responseContent = await httpResponse.Content.ReadAsStringAsync();
				throw new InvalidOperationException("API Request Failed");
			}
		}
	}

	/// <summary>
	/// Get the followers of a BlueSky User
	/// </summary>
	/// <param name="userDid">ID of the user to get followers from</param>
	/// <returns>List<BlueSkyUser></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<List<BlueSkyUser>> GetFollowersAsync(string userDid)
	{
		string result = "";
		var followers = new List<BlueSkyUser>();
		string cursor = null;

		if (_accessJwt == null)
			throw new InvalidOperationException("Must have a valid JWT token");
		if (_blueSkyHandle == null)
			throw new InvalidOperationException("Must have a valid Handle");


		using (var httpClient = new HttpClient())
		{
			httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessJwt}");

			do
			{
				string url = $"{pdsHost}/xrpc/app.bsky.graph.getFollowers?actor={userDid}&limit=100";
				url += (cursor != null ? $"&cursor={cursor}" : "");

				var response = await httpClient.GetFromJsonAsync<GetFollowersResponse>(url);

				if (response != null)
				{
					// Add current batch of followers to the list
					followers.AddRange(response.Followers);

					// Update the cursor for the next request
					cursor = response.Cursor;
				}
			} while (!string.IsNullOrEmpty(cursor));

		}

		return followers;
	}

	/// <summary>
	/// Get the users that a BlueSky User is following
	/// </summary>
	/// <param name="userDid">ID of the user</param>
	/// <returns>List<BlueSkyUser></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<List<BlueSkyUser>> GetFollowingAsync(string userDid)
	{
		string result = "";
		var followers = new List<BlueSkyUser>();
		string cursor = null;

		if (_accessJwt == null)
			throw new InvalidOperationException("Must have a valid JWT token");
		if (_blueSkyHandle == null)
			throw new InvalidOperationException("Must have a valid Handle");


		using (var httpClient = new HttpClient())
		{
			httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessJwt}");

			do
			{
				string url = $"{pdsHost}/xrpc/app.bsky.graph.getFollows?actor={userDid}&limit=100";
				url += (cursor != null ? $"&cursor={cursor}" : "");

				var response = await httpClient.GetFromJsonAsync<GetFollowsResponse>(url);

				if (response != null)
				{
					// Add current batch of followers to the list
					followers.AddRange(response.Follows);

					// Update the cursor for the next request
					cursor = response.Cursor;
				}
			} while (!string.IsNullOrEmpty(cursor));

		}

		return followers;
	}

	/// <summary>
	/// Get the DID of a BlueSky User from the handle
	/// </summary>
	/// <param name="handle">Handle of the user</param>
	/// <returns>DidResponse</returns>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<DidResponse> GetDidAsync(string handle)
	{
		var baseUrl = $"{pdsHost}/xrpc/com.atproto.identity.resolveHandle";
		using (var httpClient = new HttpClient())
		{
			// Construct the query URL
			string url = $"{baseUrl}?handle={Uri.EscapeDataString(handle)}";

			// Send a GET request
			HttpResponseMessage httpResponse = await httpClient.GetAsync(url);

			// Ensure the response is successful
			httpResponse.EnsureSuccessStatusCode();

			// Parse the response content
			string responseContent = await httpResponse.Content.ReadAsStringAsync();
			if (string.IsNullOrWhiteSpace(responseContent))
			{
				throw new InvalidOperationException("Response content is null or empty.");
			}
			// Deserialize JSON and extract the DID
			var response = JsonSerializer.Deserialize<DidResponse>(responseContent);
			return response ?? throw new InvalidOperationException("Deserialization returned null. Invalid response data.");
		}
	}

	/// <summary>
	/// Create a new session
	/// </summary>
	/// <param name="request">CreateSessionRequest</param>
	/// <returns>CreateSessionResponse</returns>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<CreateSessionResponse> CreateSessionAsync(CreateSessionRequest request)
	{
		string url = $"{pdsHost}/xrpc/com.atproto.server.createSession";

		string jsonPayload = JsonSerializer.Serialize(request);

		using (var httpClient = new HttpClient())
		{
			var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

			HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content);

			if (httpResponse.IsSuccessStatusCode)
			{
				string responseContent = await httpResponse.Content.ReadAsStringAsync();
				if (string.IsNullOrWhiteSpace(responseContent))
				{
					throw new InvalidOperationException("Response content is null or empty.");
				}
				var response = JsonSerializer.Deserialize<CreateSessionResponse>(responseContent);
				if (response == null)
				{
					throw new InvalidOperationException("Deserialization returned null. Invalid response data.");
				}
				_accessJwt = response.AccessJwt;
				_blueSkyHandle = response.Handle;
				_did = response.Did;

				return response;
			}
			else
			{
				var responseContent = await httpResponse.Content.ReadAsStringAsync();
				throw new InvalidOperationException("API Request Failed");
			}
		}
	}

	/// <summary>
	/// Create a new Post
	/// </summary>
	/// <param name="text">The text of the post</param>
	/// <returns>CreateRecordResponse</returns>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<CreateRecordResponse> CreatePostAsync(string text)
	{
		if (_accessJwt == null)
			throw new InvalidOperationException("Must have a valid JWT token");
		if (_blueSkyHandle == null)
			throw new InvalidOperationException("Must have a valid Handle");

		string url = $"{pdsHost}/xrpc/com.atproto.repo.createRecord";

		var createRecordRequest = new CreateRecordRequest
		{
			Repo = _did,
			Collection = "app.bsky.feed.post",
			Record = new Record
			{
				Text = text,
				CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
				Type = "app.bsky.feed.post"
			}
		};
		string jsonPayload = JsonSerializer.Serialize(createRecordRequest, new JsonSerializerOptions
		{
			WriteIndented = true // Optional, for pretty printing
		});

		using (var httpClient = new HttpClient())
		{
			httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessJwt}");
			var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

			HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content);

			if (httpResponse.IsSuccessStatusCode)
			{
				var responseContent = await httpResponse.Content.ReadAsStringAsync();
				if (string.IsNullOrWhiteSpace(responseContent))
				{
					throw new InvalidOperationException("Response content is null or empty.");
				}
				var result = JsonSerializer.Deserialize<CreateRecordResponse>(responseContent);
				return result ?? throw new InvalidOperationException("Deserialization returned null. Invalid response data.");
			}
			else
			{
				var responseContent = await httpResponse.Content.ReadAsStringAsync();
				throw new InvalidOperationException("API Request Failed");
			}
		}
	}
}