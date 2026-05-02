using System.ComponentModel.DataAnnotations;

public class Affirmation
{
	public int Id {get;set;}
	[Required(ErrorMessage = "Text is required")]
	[StringLength(500, ErrorMessage = "Max 500 characters allowed")]
	public string Text {get;set;}
	[Required(ErrorMessage="Category is required")]
	public int CategoryId {get;set;}
	[StringLength(100)]
	public string? Author {get;set;}
	[StringLength(50)]
	public string? Mood {get;set;}
	public bool IsActive {get;set;}
	public DateTime CreatedAt {get;set;}

	public Category Category {get;set;}

	public ICollection<AffirmationTag> AffirmationTags {get;set;}

	public ICollection<UserFavorite> UserFavorites {get;set;}
}
