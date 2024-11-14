using Qonq.BlueSky.Model;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;

namespace Qonq.BlueSky
{
    public class BlueSkyClient
    {
        private readonly string _pdsHost;
        private string _blueSkyHandle;
        private string _accessJwt;
        private string _did;

        public BlueSkyClient(string pdsHost)
        {
            _pdsHost = pdsHost;
        }

        public async Task<string> GetDid(string handle)
        {
            string didUrl = null;
            var baseUrl = "https://bsky.social/xrpc/com.atproto.identity.resolveHandle";
            using (var httpClient = new HttpClient())
            {
                // Construct the query URL
                string url = $"{baseUrl}?handle={Uri.EscapeDataString(handle)}";

                // Send a GET request
                HttpResponseMessage response = await httpClient.GetAsync(url);

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Parse the response content
                string responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize JSON and extract the DID
                var jsonResponse = JsonSerializer.Deserialize<DidResponse>(responseContent);
                didUrl = jsonResponse.Did;
            }
            return didUrl;
        }

        public async Task<CreateSessionResponse> CreateSession(CreateSessionRequest request)
        {
            CreateSessionResponse response = null;
            string url = $"{_pdsHost}/xrpc/com.atproto.server.createSession";

            string jsonPayload = JsonSerializer.Serialize(request);

            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content);

                if (httpResponse.IsSuccessStatusCode)
                {
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    response = JsonSerializer.Deserialize<CreateSessionResponse>(responseContent);

                    _accessJwt = response.AccessJwt;
                    _blueSkyHandle = response.Handle;
                    _did = response.Did;
                }
                else
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {httpResponse.StatusCode}, {responseContent}");
                }
            }
            return response;
        }

        public async Task<CreateRecordResponse> CreatePost(string text)
        {
            if (_accessJwt == null)
                throw new InvalidOperationException("Must have a valid JWT token");
            if (_blueSkyHandle == null)
                throw new InvalidOperationException("Must have a valid Handle");

            CreateRecordResponse response = null;
            string url = $"{_pdsHost}/xrpc/com.atproto.repo.createRecord";

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
                    response = JsonSerializer.Deserialize<CreateRecordResponse>(responseContent);
                }
                else
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {httpResponse.StatusCode}, {responseContent}");
                }

                return response;
            }
        }
    }
}