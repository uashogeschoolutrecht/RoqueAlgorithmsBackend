using FakeNewsBackend.Common;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FakeNewsBackend.Context;

public class SitemapContext : DbContext
{
    public DbSet<SitemapItem> SitemapItems { get; set; }
    public DbSet<SitemapGenerateProgress> GenerateProgress { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Config.GetConnectionString("DatabaseConnectionString"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var splitStringConverter = new ValueConverter<List<UrlItemDTO>, string>(
            v => string.Join("|~|", v.Select(i => $"{i.url}~|~{i.lastmod.Ticks}")), 
            v => v.Split("|~|",StringSplitOptions.None)
                .Select(str => new UrlItemDTO()
                    {
                        lastmod = new DateTime(long.Parse(
                            str.Split("~|~", StringSplitOptions.None).Length > 1 ?
                            str.Split("~|~", StringSplitOptions.None)[1] : "0"))
                            .ToUniversalTime(),
                        url = str.Split("~|~", StringSplitOptions.None)[0]
                    }
                ).ToList());
        modelBuilder.Entity<SitemapGenerateProgress>()
            .Property(s => s.LinksVisited)
            .HasConversion(splitStringConverter);
        modelBuilder.Entity<SitemapGenerateProgress>()
            .Property(s => s.LinksOnWebsite)
            .HasConversion(splitStringConverter);
        modelBuilder.Entity<SitemapGenerateProgress>()
            .Property(s => s.LinksWithArticles)
            .HasConversion(splitStringConverter);
        
        modelBuilder.Entity<SitemapItem>()
            .HasKey(item => new { item.websiteId, item.url });
        modelBuilder.HasDefaultSchema("public");
        base.OnModelCreating(modelBuilder);
    }
}