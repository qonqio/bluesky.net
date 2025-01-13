using Qonq.BlueSky.Model;
namespace Qonq.BlueSky.Tests;

public class APITests
{
    private const string PdsHost = "https://bsky.social";
    private readonly BlueSkyClient _client;
    private readonly string _handle;
    private readonly string _password;
    private string _did;

    public APITests()
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
        var didResponse = await _client.GetDidAsync(_handle);

        Assert.NotNull(didResponse);
        Assert.NotNull(didResponse.Did);
        Assert.NotEmpty(didResponse.Did);
        Assert.Equal(32, didResponse.Did.Length);
		_did = didResponse.Did;

	}

    [Fact]
    public async Task StartSession()
    {
        var sessionRequest = new CreateSessionRequest()
        {
            Identifier = _handle,
            Password = _password
        };

        var sessionResponse = await _client.CreateSessionAsync(sessionRequest);

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

        var sessionResponse = await _client.CreateSessionAsync(sessionRequest);

        Assert.NotNull(sessionResponse);
        Assert.NotNull(sessionResponse.AccessJwt);
        Assert.NotEmpty(sessionResponse.AccessJwt);

        var text = "Beep, Beep, Boop!";

        var postResponse = await _client.CreatePostAsync(text);

        Assert.NotNull(postResponse);

        Assert.NotNull(postResponse.Uri);
        Assert.NotEmpty(postResponse.Uri);

        Assert.NotNull(postResponse.Cid);
        Assert.NotEmpty(postResponse.Cid);
    }

	[Fact]
	public async Task GetFollowers()
	{
		var sessionRequest = new CreateSessionRequest()
		{
			Identifier = _handle,
			Password = _password
		};
		var sessionResponse = await _client.CreateSessionAsync(sessionRequest);
		var followers = await _client.GetFollowersAsync(_did);
		Assert.NotNull(followers);
		Assert.NotEmpty(followers);
	}

	[Fact]
	public async Task GetFollowing()
	{
		var sessionRequest = new CreateSessionRequest()
		{
			Identifier = _handle,
			Password = _password
		};
		var sessionResponse = await _client.CreateSessionAsync(sessionRequest);
		var following = await _client.GetFollowingAsync(_did);
		Assert.NotNull(following);
		Assert.NotEmpty(following);
	}

    [Fact]
    public async Task FollowUser()
    {
        var sessionRequest = new CreateSessionRequest()
        {
            Identifier = _handle,
            Password = _password
        };
        var sessionResponse = await _client.CreateSessionAsync(sessionRequest);
        Assert.NotNull(sessionResponse);
        Assert.NotNull(sessionResponse.AccessJwt);
        Assert.NotEmpty(sessionResponse.AccessJwt);
        var followResponse = await _client.FollowUserAsync("did:bluesky:z6Mk3ZQ5Q7Qqz");
    }
}