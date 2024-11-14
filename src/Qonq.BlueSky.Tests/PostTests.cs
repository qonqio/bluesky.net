using Qonq.BlueSky;
using Qonq.BlueSky.Model;
using System.Reflection.Metadata;

namespace Qonq.BlueSky.Tests
{
    public class PostTests
    {
        [Fact]
        public async Task GetDid()
        {
            var pdsHost = "https://bsky.social";
            var blueSkyClient = new BlueSkyClient(pdsHost);

            var handle = Environment.GetEnvironmentVariable("BLUESKY_HANDLE");

            var didUrl = await blueSkyClient.GetDid(handle);

            Assert.NotNull(didUrl);
            Assert.NotEmpty(didUrl);
            Assert.Equal(32, didUrl.Length);
        }

        [Fact]
        public async Task StartSession()
        {
            var pdsHost = "https://bsky.social";
            var blueSkyClient = new BlueSkyClient(pdsHost);

            var handle = Environment.GetEnvironmentVariable("BLUESKY_HANDLE");
            var password = Environment.GetEnvironmentVariable("BLUESKY_PASSWORD");

            var sessionRequest = new CreateSessionRequest()
            {
                Identifier = handle,
                Password = password
            };

            var sessionResponse = await blueSkyClient.CreateSession(sessionRequest);

            Assert.NotNull(sessionResponse);
            Assert.NotNull(sessionResponse.AccessJwt);
            Assert.NotEmpty(sessionResponse.AccessJwt);
        }

        public async Task PostSomething()
        {
            var pdsHost = "https://bsky.social";
            var blueSkyClient = new BlueSkyClient(pdsHost);

            var handle = Environment.GetEnvironmentVariable("BLUESKY_HANDLE");
            var password = Environment.GetEnvironmentVariable("BLUESKY_PASSWORD");

            var sessionRequest = new CreateSessionRequest()
            {
                Identifier = handle,
                Password = password
            };

            var sessionResponse = await blueSkyClient.CreateSession(sessionRequest);

            Assert.NotNull(sessionResponse);
            Assert.NotNull(sessionResponse.AccessJwt);
            Assert.NotEmpty(sessionResponse.AccessJwt);

            var text = "Beep, Beep, Boop!";

            var postResponse = await blueSkyClient.CreatePost(text);

            Assert.NotNull(postResponse);

            Assert.NotNull(postResponse.Uri);
            Assert.NotEmpty(postResponse.Uri);

            Assert.NotNull(postResponse.Cid);
            Assert.NotEmpty(postResponse.Cid);
        }
    }
}