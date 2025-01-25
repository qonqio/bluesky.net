using Qonq.BlueSky.Helper;
using Qonq.BlueSky.Helper.Extensions;
using Qonq.BlueSky.Helper.Strings;
using Qonq.BlueSky.Model;
using System;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Qonq.BlueSky
{
	public class BlueSkyClient(string pdsHost)
	{
        private const string feedPost = "app.bsky.feed.post";

        private string _blueSkyHandle;
		private string _accessJwt;
		private string _did;

		/// <summary>
		/// Get a BlueSky User by handle
		/// </summary>
		/// <param name="handle">The @ handle</param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<BlueSkyUser?> GetUserAsync(string handle)
		{
	
			if (_accessJwt == null)
				throw new InvalidOperationException("Must have a valid JWT token");
			if (_blueSkyHandle == null)
				throw new InvalidOperationException("Must have a valid Handle");


            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessJwt}");

            string url = $"{pdsHost}/xrpc/app.bsky.actor.getProfile?actor={handle}";

            var response = await httpClient.GetFromJsonAsync<BlueSkyUser>(url);

            return response;
        }

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
				Record = new RecordValue
				{
					Subject = userDid,
					CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
				}
			};

			string jsonPayload = JsonSerializer.Serialize(createRecordRequest, new JsonSerializerOptions
			{
				WriteIndented = true // Optional, for pretty printing
			});

            using HttpClient httpClient = new();
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


		/// <summary>
		/// Follow a BlueSky User
		/// </summary>
		/// <param name="userDid">The ID of the user you want to follow</param>
		/// <returns>CreateRecordResponse</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<CreateRecordResponse> UnfollowUserAsync(string userDid)
		{
			if (_accessJwt == null)
				throw new InvalidOperationException("Must have a valid JWT token");
			if (_blueSkyHandle == null)
				throw new InvalidOperationException("Must have a valid Handle");

			var followCollectionName = "app.bsky.graph.follow";
			var allFollowRecords = await GetAllFollowRecordsAsync(followCollectionName);
			var followRecordToDelete = allFollowRecords.FirstOrDefault(f => f.Value.Subject == userDid);

			if (followRecordToDelete == null)
				throw new InvalidOperationException("User Not Followed");

			string url = $"{pdsHost}/xrpc/com.atproto.repo.deleteRecord";

			var deleteRecordRequest = new DeleteRecordRequest
			{
				Repo = _did,
				Collection = "app.bsky.graph.follow",
				RecordKey = followRecordToDelete.Uri.Split('/').Last()
			};

			string jsonPayload = JsonSerializer.Serialize(deleteRecordRequest, new JsonSerializerOptions
			{
				WriteIndented = true // Optional, for pretty printing
			});

            using HttpClient httpClient = new();
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

		//
		public async Task<List<Record>> GetAllFollowRecordsAsync(string collection)
		{
			if (_accessJwt == null)
				throw new InvalidOperationException("Must have a valid JWT token");

			string baseUrl = $"{pdsHost}/xrpc/com.atproto.repo.listRecords";
			var allRecords = new List<Record>();
			string? cursor = null;

			using (HttpClient httpClient = new())
			{
				httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessJwt}");

				do
				{
					string url = $"{baseUrl}?repo={_did}&collection={collection}";
					if (!string.IsNullOrEmpty(cursor))
					{
						url += $"&cursor={Uri.EscapeDataString(cursor)}";
					}

					HttpResponseMessage httpResponse = await httpClient.GetAsync(url);

					if (httpResponse.IsSuccessStatusCode)
					{
						var responseContent = await httpResponse.Content.ReadAsStringAsync();
						var result = JsonSerializer.Deserialize<ListRecordsResponse>(responseContent);

						if (result != null)
						{
							allRecords.AddRange(result.Records);
							cursor = result.Cursor; // Update cursor for the next iteration
						}
						else
						{
							cursor = null; // Stop if no more data
						}
					}
					else
					{
						var responseContent = await httpResponse.Content.ReadAsStringAsync();
						throw new InvalidOperationException($"API Request Failed: {responseContent}");
					}
				} while (!string.IsNullOrEmpty(cursor));
			}

			return allRecords;
		}

		/// <summary>
		/// Get the followers of a BlueSky User
		/// https://docs.bsky.app/docs/api/app-bsky-graph-get-followers
		/// </summary>
		/// <param name="userDid">ID of the user to get followers from</param>
		/// <returns>List<BlueSkyUser></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<List<BlueSkyUser>> GetFollowersAsync(string userDid)
		{
            List<BlueSkyUser> followers = new();
			string cursor = null;

			if (_accessJwt == null)
				throw new InvalidOperationException("Must have a valid JWT token");
			if (_blueSkyHandle == null)
				throw new InvalidOperationException("Must have a valid Handle");


			using (HttpClient httpClient = new())
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
			var followers = new List<BlueSkyUser>();
			string cursor = null;

			if (_accessJwt == null)
				throw new InvalidOperationException("Must have a valid JWT token");
			if (_blueSkyHandle == null)
				throw new InvalidOperationException("Must have a valid Handle");


			using (HttpClient httpClient = new())
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
		/// https://docs.bsky.app/docs/api/com-atproto-identity-resolve-handle
		/// </summary>
		/// <param name="handle">Handle of the user</param>
		/// <returns>DidResponse</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<DidResponse> GetDidAsync(string handle)
		{
			var baseUrl = $"{pdsHost}/xrpc/com.atproto.identity.resolveHandle";
            using HttpClient httpClient = new();
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

        /// <summary>
        /// Create a new Post without an image
        /// </summary>
        /// <param name="text">The text of the post</param>
        /// <returns>CreateRecordResponse</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<CreateRecordResponse> CreatePostAsync(string text)
		{
			return	 await CreatePostAsync(text);

        }

        /// <summary>
        /// Create a new Post
        /// </summary>
        /// <param name="text">The text of the post</param>
        /// <param name="images">Array of Base64 image string and Alt text image</param>
        /// <returns>CreateRecordResponse</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<CreateRecordResponse> CreatePostAsync(string text, (string, string?)[]? images)
        {
			if(images!=null && images.Length> 4)
			{
                throw new InvalidOperationException("Max 4 images are allowed");
            }
            if (_accessJwt == null)
                throw new InvalidOperationException("Must have a valid JWT token");
            if (_blueSkyHandle == null)
                throw new InvalidOperationException("Must have a valid Handle");

            string url = $"{pdsHost}/xrpc/com.atproto.repo.createRecord";

            UnicodeString utf16 = new UnicodeString(text);

            var facts = Facets.DetectFacets(utf16);
            Embed embed = new Embed();
            embed.Images = new List<Image>();
            embed.Type = "app.bsky.embed.images";

            if (images != null && images.Length>0)
            {
                UploadBlobResponse imageResponse;
				Image img;

                foreach (var image in images)
				{
                    imageResponse = await UploadBlobAsync(image.Item1);

                    img = new Image
					{
                        alt = image.Item2 != null ? image.Item2 : "Image",
                        image = imageResponse.blob,
                        aspectRatio = new AspectRatio { width = 640, height = 480 }
                    };
                    embed.Images.Add(img);
                }	

            }
            CreateRecordRequest createRecordRequest = new CreateRecordRequest
            {
                Repo = _did,
                Collection = feedPost,
                Record = new RecordValue
                {
                    Text = text,
                    CreatedAt = DateTime.UtcNow.toISOstring(),
                    Type = feedPost,
                    Facets = facts,
                    Embed = embed

                }
            };
            string jsonPayload = JsonSerializer.Serialize(createRecordRequest, new JsonSerializerOptions
            {
                WriteIndented = true // Optional, for pretty printing
            });

            using HttpClient httpClient = new();
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

        /// <summary>
        /// Create a new Post with an image
        /// </summary>
        /// <param name="text">The text of the post</param>
        /// <param name="image">Base64 string</param>
		/// <param name="alt">alt text of image</param>
        /// <returns>CreateRecordResponse</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<CreateRecordResponse> CreatePostAsync(string text, string? image, string? alt)
		{
			if (_accessJwt == null)
				throw new InvalidOperationException("Must have a valid JWT token");
			if (_blueSkyHandle == null)
				throw new InvalidOperationException("Must have a valid Handle");

			string url = $"{pdsHost}/xrpc/com.atproto.repo.createRecord";

            UnicodeString utf16 = new UnicodeString(text);
			
            var facts =Facets.DetectFacets(utf16);

			UploadBlobResponse imageResponse;
			Embed embed = new Embed();
			embed.Images = new List<Image>();
			embed.Type = "app.bsky.embed.images";

            if (!string.IsNullOrEmpty(image))
			{
				 imageResponse = await UploadBlobAsync(image);

				Image img = new Image
				{
					alt = alt != null ? alt : "Image",
					image = imageResponse.blob,
					aspectRatio = new AspectRatio { width = 640, height = 480 }
				};
                embed.Images.Add(img);
            }

            CreateRecordRequest createRecordRequest = new CreateRecordRequest
			{
				Repo = _did,
				Collection = feedPost,
				Record = new RecordValue
				{
					Text = text,
					CreatedAt = DateTime.UtcNow.toISOstring(),
					Type = feedPost,
					Facets = facts,
					Embed =  embed
					
                }
			};
			string jsonPayload = JsonSerializer.Serialize(createRecordRequest, new JsonSerializerOptions
			{
				WriteIndented = true // Optional, for pretty printing
			});

            using HttpClient httpClient = new();
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

        /// <summary>
        /// Create a new Post with Website card embeds
        /// </summary>
        /// <param name="text">The text of the post</param>
        /// <param name="image">Base64 string</param>
        /// <param name="uri">External Uri</param>
        /// <param name="title">Title of the Website card</param
        /// <param name="description">Description of the Website card</param>
        /// <returns>CreateRecordResponse</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<CreateRecordResponse> CreatePostExternalAsync(string text, string image, string uri, string title, string description)
        {
            if (_accessJwt == null)
                throw new InvalidOperationException("Must have a valid JWT token");
            if (_blueSkyHandle == null)
                throw new InvalidOperationException("Must have a valid Handle");

            string url = $"{pdsHost}/xrpc/com.atproto.repo.createRecord";

            UnicodeString utf16 = new UnicodeString(text);

            var facts = Facets.DetectFacets(utf16);

            UploadBlobResponse imageResponse;

            Embed embed = new Embed();
            embed.Type = "app.bsky.embed.external";

            embed.External =
                new External
                {
                    Uri = uri,// "https://github.com",
                    Title = title,// "GitHub",
                    Description = description//"GitHub is a development platform inspired by the way you work. From open source to business, you can host and review code, manage projects, and build software alongside millions of other developers."
                };
        
            if (!string.IsNullOrEmpty(image))
            {
                imageResponse = await UploadBlobAsync(image);
                embed.External.Thumb = new Thumb
                {
                    type = "blob",
                    @ref = new Ref { link = imageResponse.blob.@ref.link },
                    MimeType = imageResponse.blob.mimeType,
                    Size = imageResponse.blob.size
                };
            }

            CreateRecordRequest createRecordRequest = new CreateRecordRequest
            {
                Repo = _did,
                Collection = feedPost,
                Record = new RecordValue
                {
                    Text = text,
                    CreatedAt = DateTime.UtcNow.toISOstring(),
                    Type = feedPost,
                    Facets = facts,
                    Embed = embed

                }
            };
            string jsonPayload = JsonSerializer.Serialize(createRecordRequest, new JsonSerializerOptions
            {
                WriteIndented = true // Optional, for pretty printing
            });

            using HttpClient httpClient = new();
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

            using HttpClient httpClient = new();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image">data:image/png;base64,...</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>
        ///data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/6l9lAAAAABJRU5ErkJggg== 
        /// </remarks>
        public async Task<UploadBlobResponse> UploadBlobAsync(string image)
        {
            if (_accessJwt == null)
                throw new InvalidOperationException("Must have a valid JWT token");
            if (_blueSkyHandle == null)
                throw new InvalidOperationException("Must have a valid Handle");

            string url = $"{pdsHost}/xrpc/com.atproto.repo.uploadBlob";


            BlobConverter blob = new BlobConverter();
			byte[] binaryData = blob.ConvertDataURIToByteArray(image);


            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessJwt}");
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "image/png");
           


            var content = new ByteArrayContent(binaryData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    throw new InvalidOperationException("Response content is null or empty.");
                }
                var result = JsonSerializer.Deserialize<UploadBlobResponse>(responseContent);
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
