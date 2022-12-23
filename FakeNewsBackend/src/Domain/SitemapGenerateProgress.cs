using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FakeNewsBackend.Domain.DTO;

namespace FakeNewsBackend.Domain;

public class SitemapGenerateProgress
{
    [Key]
    public int WebsiteId { get; set; }
    [AllowNull]
    public List<UrlItemDTO> LinksVisited { get; set; }
    [AllowNull]
    public List<UrlItemDTO> LinksOnWebsite { get; set; }
    [AllowNull]
    public List<UrlItemDTO> LinksWithArticles { get; set; }
    
}