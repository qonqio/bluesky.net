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

            var handle = "";

            var didUrl = await blueSkyClient.GetDid(handle);

            Assert.NotNull(didUrl);
            Assert.NotEmpty(didUrl);
            Assert.Equal(32, didUrl.Length);
        }

        [Fact]
        public async Task Test1()
        {
            var pdsHost = "https://bsky.social";
            var blueSkyClient = new BlueSkyClient(pdsHost);

            var sessionRequest = new CreateSessionRequest()
            {
                Identifier = "",
                Password = ""
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