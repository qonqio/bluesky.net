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

    public static string GetBase64Image(string relativePath)
    {
        // Resolve the full path
        string fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);

        // Read the bytes and convert to Base64
        byte[] imageBytes = File.ReadAllBytes(fullPath);
        string base64String = Convert.ToBase64String(imageBytes);

        return base64String;
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

        var imageContents = GetBase64Image("./Samples/images/bluesky-anakin.jpg");
        var image1 = new ImageContent()
        {
            Base64EncodedContent = imageContents,
            MimeType = "image/jpg"
        };

        var postResponse = await _client.CreatePostAsync(text, image1, "Test");

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

        var image1 = new ImageContent()
        {
            AlternativeText = "Anakin Likes to Test in Production",
            Base64EncodedContent = GetBase64Image("./Samples/images/bluesky-anakin.jpg"),
            MimeType = "image/jpg"
        };
        var image2 = new ImageContent()
        {
            AlternativeText = "Testing in Production...Just more Fun",
            Base64EncodedContent = GetBase64Image("./Samples/images/bluesky-distractedboyfriend.jpg"),
            MimeType = "image/jpg"
        };
        var image3 = new ImageContent()
        {
            AlternativeText = "Drake Likes Testing in Production",
            Base64EncodedContent = GetBase64Image("./Samples/images/bluesky-drake.jpg"),
            MimeType = "image/jpg"
        };
        var image4 = new ImageContent()
        {
            AlternativeText = "Real Developers Test in Production",
            Base64EncodedContent = GetBase64Image("./Samples/images/bluesky-galaxybrain.jpg"),
            MimeType = "image/jpg"
        };

        var imageList = new List<ImageContent>();
        imageList.Add(image1);
        imageList.Add(image2);
        imageList.Add(image3);
        imageList.Add(image4);

        var postResponse = await _client.CreatePostAsync(text, imageList);

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

        var image1 = new ImageContent()
        {
            AlternativeText = "Image 1",
            Base64EncodedContent = GetBase64Image("./Samples/images/bluesky-anakin.jpg"),
            MimeType = "image/jpg"
        };
        var allFollowRecords = await _client.UploadBlobAsync(image1);
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