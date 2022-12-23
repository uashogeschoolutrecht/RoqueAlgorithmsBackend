using FakeNewsBackend.Common;
using FakeNewsBackend.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FakeNewsBackend.Context;

public class RobotRulesContext : DbContext
{
    public DbSet<RobotRules> robotRules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Config.GetConnectionString("DatabaseConnectionString"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var splitStringConverter = new ValueConverter<List<string>, string>(
            v => string.Join(";", v), 
            v => v.Split(new[] { ';' }).ToList());
        modelBuilder.Entity<RobotRules>()
            .Property(nameof(RobotRules.AllowedLinks))
            .HasConversion(splitStringConverter);
        modelBuilder.Entity<RobotRules>()
            .Property(nameof(RobotRules.DisallowedLinks))
            .HasConversion(splitStringConverter);
        modelBuilder.HasDefaultSchema("public");
        base.OnModelCreating(modelBuilder);
    }
}