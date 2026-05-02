public class UserFavorite
{
	public int Id {get;set;}
	public string UserId {get;set;}
	public int AffirmationId {get;set;}
	public DateTime CreatedAt {get;set;}
	public Affirmation Affirmation {get;set;}

}
