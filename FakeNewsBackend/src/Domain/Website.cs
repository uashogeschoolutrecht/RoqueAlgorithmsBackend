using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeNewsBackend.Domain;

public class Website
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public int NumberOfArticles { get; set; }
    public int NumberOfArticlesScraped { get; set; }
    
    [NotMapped]
    public Sitemap? Sitemap { get; set; }

    [NotMapped]
    public RobotRules? Rules { get; set; }
    public bool HasSitemap { get; set; }
    public bool HasRules { get; set; }
    public bool IsWhitelisted { get; set; }
    public bool ShouldNotBeScraped { get; set; }
    [ForeignKey("ProgressId")]
    public WebsiteProgress? Progress { get; set; }

    public void AddRules(RobotRules rules)
    {
        this.Rules = rules;
    }

    public RobotRules GetRules()
    {
        return Rules;
    }

    public bool IsLinkAllowed(string link)
    {
        if (Rules == null)
            return true;
        return Rules.IsLinkAllowed(link);
    }

    public override string ToString()
    {
        return $"id: {Id}\n" +
               $"name: {Name}\n" +
               $"url: {Url}\n" +
               $"number of articles: {NumberOfArticles}\n" +
               $"number of articles scraped: {NumberOfArticlesScraped}\n" +
               $"Rules: {Rules ?? null}"+
               $"HasRules: {HasRules}"+
               $"HasSitemap: {HasSitemap}"+
               $"Progres: {Progress.CurrentlyWorkingOn}";
    }

    protected bool Equals(Website other)
    {
        return Name == other.Name && Url == other.Url;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Website)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Url);
    }
}