namespace Qonq.BlueSky.Model;

public class GetFollowsResponse
{
	public List<BlueSkyUser> Follows { get; set; }
	public string Cursor { get; set; }
}
