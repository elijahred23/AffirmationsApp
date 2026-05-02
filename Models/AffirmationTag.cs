public class AffirmationTag
{
	public int AffirmationId {get;set;}
	public int TagId {get;set;}
	public DateTime CreatedAt {get;set;}
	public Affirmation Affirmation {get;set;}
	public Tag Tag {get;set;}
}
