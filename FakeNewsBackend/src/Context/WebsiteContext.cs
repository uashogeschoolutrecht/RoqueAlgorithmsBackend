using FakeNewsBackend.Common;
using FakeNewsBackend.Domain;
using Microsoft.EntityFrameworkCore;


namespace FakeNewsBackend.Context;

public class WebsiteContext : DbContext
{
    public DbSet<Website> Websites { get; set; }
    public DbSet<WebsiteProgress> Progress { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Config.GetConnectionString("DatabaseConnectionString"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        base.OnModelCreating(modelBuilder);
    }
}