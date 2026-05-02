using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) {} 

	public DbSet<Category> Categories {get;set;}
	public DbSet<Affirmation> Affirmations {get;set;}
	public DbSet<Tag> Tags { get;set;}
	public DbSet<AffirmationTag> AffirmationTags {get;set;}
	public DbSet<UserFavorite> UserFavorites {get;set;}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<AffirmationTag>()
			.HasKey(at => new { at.AffirmationId, at.TagId});

		modelBuilder.Entity<AffirmationTag>()
			.HasOne(at => at.Affirmation)
			.WithMany(a => a.AffirmationTags)
			.HasForeignKey(at => at.AffirmationId);

		modelBuilder.Entity<AffirmationTag>()
			.HasOne(at => at.Tag)
			.WithMany(t => t.AffirmationTags)
			.HasForeignKey(at => at.TagId);

	}
}
