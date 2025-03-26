using Microsoft.Extensions.Configuration;
using Qonq.BlueSky.Model;
namespace Qonq.BlueSky.Tests;

public class APITests
{
    private IConfiguration Configuration { get; }
    private const string PdsHost = "https://bsky.social";
    private readonly BlueSkyClient _client;
    private readonly string _handle;
    private readonly string _password;

    public APITests()
    {
        _client = new BlueSkyClient(PdsHost);

        Configuration = new ConfigurationBuilder()
            .AddUserSecrets<APITests>()
            .AddEnvironmentVariables()
            .Build();

        Console.WriteLine(Configuration);
        _handle =  Configuration["BLUESKY_HANDLE"];
        if (_handle == null)
            throw new InvalidOperationException("Must specify BlueSky Handle");

        _password = Configuration["BLUESKY_PASSWORD"];
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
    public async Task PostSomethingWithImage()
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

        var text = "Image, Image, Boop!";

        var postResponse = await _client.CreatePostAsync(text, "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/6l9lAAAAABJRU5ErkJggg==", "Test");

        Assert.NotNull(postResponse);

        Assert.NotNull(postResponse.Uri);
        Assert.NotEmpty(postResponse.Uri);

        Assert.NotNull(postResponse.Cid);
        Assert.NotEmpty(postResponse.Cid);
    }

    [Fact]
    public async Task PostSomethingWithImages()
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

        var text = "Image, Image, Boop! https://github.com";
        (string, string?)[]  images = new (string, string?)[] { 
            ("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/6l9lAAAAABJRU5ErkJggg==", "Image 1"),
            ("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/6l9lAAAAABJRU5ErkJggg==", "Image 2"),
            ("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/6l9lAAAAABJRU5ErkJggg==", "Image 3"),
            ("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/6l9lAAAAABJRU5ErkJggg==", "Image 4")
        };

        var postResponse = await _client.CreatePostAsync(text, images);

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
        var myDid = sessionResponse.Did;
        var followers = await _client.GetFollowersAsync(myDid);
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
        var myDid = sessionResponse.Did;
        var following = await _client.GetFollowingAsync(myDid);
		Assert.NotNull(following);
		Assert.NotEmpty(following);
    }

    [Fact]
    public async Task GetUserByHandle()
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
        var userProfile = await _client.GetUserAsync("blueskydotnet.bsky.social");

        Assert.NotNull(userProfile);
    }

    [Fact]
    public async Task GetUserByDid()
    {
        var sessionRequest = new CreateSessionRequest()
        {
            Identifier = _handle,
            Password = _password
        };
        var sessionResponse = await _client.CreateSessionAsync(sessionRequest);
        var myDid = sessionResponse.Did;
        Assert.NotNull(sessionResponse);
        Assert.NotNull(sessionResponse.AccessJwt);
        Assert.NotEmpty(sessionResponse.AccessJwt);
        var userProfile = await _client.GetUserAsync(myDid);

        Assert.NotNull(userProfile);
    }

    [Fact]
    public async Task GetFollowRecords()
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
        var followCollectionName = "app.bsky.graph.follow";
        var allFollowRecords = await _client.GetAllFollowRecordsAsync(followCollectionName);
        Assert.NotNull(allFollowRecords);
    }

    [Fact]
    public async Task UploadBlob()
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
        var imageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/6l9lAAAAABJRU5ErkJggg==";
        var allFollowRecords = await _client.UploadBlobAsync(imageBase64);
        Assert.NotNull(allFollowRecords);
    }

    [Fact]
    public async Task FollowUser()
    {
        var sessionRequest = new CreateSessionRequest()
        {
            Identifier = _handle,
            Password = _password
        };
        var kevinBacon = "realkevinbacon.bsky.social";
        var blueSkySocial = "bsky.app";
        var sessionResponse = await _client.CreateSessionAsync(sessionRequest);
        var userToFollow = await _client.GetUserAsync(blueSkySocial);
        var userToFollowDid = userToFollow.Did;
        Assert.NotNull(sessionResponse);
        Assert.NotNull(sessionResponse.AccessJwt);
        Assert.NotEmpty(sessionResponse.AccessJwt);

        // Am I following Kevin Bacon?
        var followCollectionName = "app.bsky.graph.follow";
        var followBeforeAdd = await _client.GetAllFollowRecordsAsync(followCollectionName);
        var existenceBeforeAdd = followBeforeAdd.Any(f => f.Value.Subject == userToFollowDid);
        Assert.False(existenceBeforeAdd); // NO

        // Follow Kevin Bacon
        var followResponse = await _client.FollowUserAsync(userToFollowDid);

        // Am I following Kevin Bacon?
        var followAfterAdd = await _client.GetAllFollowRecordsAsync(followCollectionName);
        var existenceAfterAdd = followAfterAdd.Any(f => f.Value.Subject == userToFollowDid);
        Assert.True(existenceAfterAdd); // YES

        // UN-Follow Kevin Bacon
        var unfollowResponse = await _client.UnfollowUserAsync(userToFollowDid);

        // Am I following Kevin Bacon?
        var followAfterDelete = await _client.GetAllFollowRecordsAsync(followCollectionName);
        var existenceAfterDelete = followAfterDelete.Any(f => f.Value.Subject == userToFollowDid);
        Assert.False(existenceAfterDelete); // NO
    }
}