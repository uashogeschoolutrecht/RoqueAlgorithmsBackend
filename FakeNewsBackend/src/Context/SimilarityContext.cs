using FakeNewsBackend.Common;
using FakeNewsBackend.Domain;
using Microsoft.EntityFrameworkCore;

namespace FakeNewsBackend.Context;

public class SimilarityContext : DbContext
{
    public DbSet<Similarity> Similarities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Config.GetConnectionString("DatabaseConnectionString"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Similarity>().HasKey(sim =>
            new
            {
                sim.FoundWebsiteId, 
                sim.OriginalWebsiteId, 
                sim.UrlToFoundArticle, 
                sim.UrlToOriginalArticle
            });
        modelBuilder.Entity<Similarity>()
            .Property(c => c.FoundLanguage)
            .HasConversion<string>();
        modelBuilder.Entity<Similarity>()
            .Property(c => c.OriginalLanguage)
            .HasConversion<string>();
        modelBuilder.HasDefaultSchema("public");
        base.OnModelCreating(modelBuilder);
    }

}