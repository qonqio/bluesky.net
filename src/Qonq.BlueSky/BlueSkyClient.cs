using Qonq.BlueSky.Model;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;

namespace Qonq.BlueSky
{
    public class BlueSkyClient(string pdsHost)
    {
        private string _blueSkyHandle;
        private string _accessJwt;
        private string _did;

        public async Task<DidResponse> GetDid(string handle)
        {
            var baseUrl = "https://bsky.social/xrpc/com.atproto.identity.resolveHandle";
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
                var response = JsonSerializer.Deserialize<DidResponse>(responseContent) ;
                return response ?? throw new InvalidOperationException("Deserialization returned null. Invalid response data.");
            }
        }

        public async Task<CreateSessionResponse> CreateSession(CreateSessionRequest request)
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
                    if(response == null)
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

        public async Task<CreateRecordResponse> CreatePost(string text)
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
}