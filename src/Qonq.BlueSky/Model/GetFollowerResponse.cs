namespace Qonq.BlueSky.Model;

public class GetFollowersResponse
{
	public List<BlueSkyUser> Followers { get; set; }
	public string Cursor { get; set; }
}
