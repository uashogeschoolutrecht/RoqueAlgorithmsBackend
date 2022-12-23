using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;

namespace FakeNewsBackend.Service.Interface;

public interface ISitemapService : IService<Sitemap>
{
    /// <summary>
    /// Save Multiple <see cref="SitemapItem"/> objects in the database.
    /// </summary>
    /// <param name="items">A <see cref="IEnumerable{SitemapItem}"/> with the objects to save.</param>
    public void SaveMultipleSitemaps(IEnumerable<SitemapItem> items);
    
    /// <summary>
    /// Retrieve a <see cref="Sitemap"/> object from the database given a <paramref name="id"/>,
    /// </summary>
    /// <param name="id">The id of the <see cref="Website"/>.</param>
    /// <returns>The <see cref="Sitemap"/> found with the <paramref name="id"/>.</returns>
    public Sitemap GetSitemapByWebsiteId(int id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void UpdateItem(SitemapItem item);
    public bool ItemsExistsWithId(int websiteId);
    public List<UrlItemDTO> GetSitmapItemDtos(int websiteId);
    
    
    public void SaveGenerateProgress(SitemapGenerateProgress prog);
    public void DeleteGenerateProgress(SitemapGenerateProgress prog);
    public bool ProgressExists(int websiteId);
    public SitemapGenerateProgress GetProgress(int websiteId);
}