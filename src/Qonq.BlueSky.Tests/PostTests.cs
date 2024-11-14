using Qonq.BlueSky;
using Qonq.BlueSky.Model;
using System.Reflection.Metadata;

namespace Qonq.BlueSky.Tests
{
    public class PostTests
    {
        private const string PdsHost = "https://bsky.social";
        private readonly BlueSkyClient _client;
        private readonly string _handle;
        private readonly string _password;

        public PostTests()
        {
            _client = new BlueSkyClient(PdsHost);

            _handle = Environment.GetEnvironmentVariable("BLUESKY_HANDLE");
            if (_handle == null)
                throw new InvalidOperationException("Must specify BlueSky Handle");

            _password = Environment.GetEnvironmentVariable("BLUESKY_PASSWORD");
            if (_password == null)
                throw new InvalidOperationException("Must specify BlueSky Password");
        }

        [Fact]
        public async Task GetDid()
        {
            var didResponse = await _client.GetDid(_handle);

            Assert.NotNull(didResponse);
            Assert.NotNull(didResponse.Did);
            Assert.NotEmpty(didResponse.Did);
            Assert.Equal(32, didResponse.Did.Length);
        }

        [Fact]
        public async Task StartSession()
        {
            var sessionRequest = new CreateSessionRequest()
            {
                Identifier = _handle,
                Password = _password
            };

            var sessionResponse = await _client.CreateSession(sessionRequest);

            Assert.NotNull(sessionResponse);
            Assert.NotNull(sessionResponse.AccessJwt);
            Assert.NotEmpty(sessionResponse.AccessJwt);
        }

        [Fact]
        public async Task PostSomething()
        {
            var sessionRequest = new CreateSessionRequest()
            {
                Identifier = _handle,
                Password = _password
            };

            var sessionResponse = await _client.CreateSession(sessionRequest);

            Assert.NotNull(sessionResponse);
            Assert.NotNull(sessionResponse.AccessJwt);
            Assert.NotEmpty(sessionResponse.AccessJwt);

            var text = "Beep, Beep, Boop!";

            var postResponse = await _client.CreatePost(text);

            Assert.NotNull(postResponse);

            Assert.NotNull(postResponse.Uri);
            Assert.NotEmpty(postResponse.Uri);

            Assert.NotNull(postResponse.Cid);
            Assert.NotEmpty(postResponse.Cid);
        }
    }
}