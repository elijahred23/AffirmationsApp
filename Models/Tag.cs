public class Tag
{
	public int Id {get;set;}
	public string Name {get;set;}

	public ICollection<AffirmationTag> AffirmationTags {get;set;}
}
