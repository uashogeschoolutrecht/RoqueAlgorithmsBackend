namespace FakeNewsBackend.Domain;

public class SitemapItem
{
    public int websiteId { get; set; }
    public string url { get; set; }
    public DateTime date { get; set; }
    public bool scraped { get; set; }

    protected bool Equals(SitemapItem other)
    {
        return websiteId == other.websiteId && url == other.url;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SitemapItem)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(websiteId, url);
    }
}