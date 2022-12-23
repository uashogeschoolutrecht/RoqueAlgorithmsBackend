using System.ComponentModel.DataAnnotations;

namespace FakeNewsBackend.Domain;

public class Sitemap
{
    [Key]
    public int WebsiteId { get; set; }
    public IEnumerable<SitemapItem> links { get; set; }

    public int getTotalLinks()
    {
        return links.Count();
    }
}