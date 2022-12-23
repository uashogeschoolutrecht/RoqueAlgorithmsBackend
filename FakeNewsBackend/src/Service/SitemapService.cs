using FakeNewsBackend.Context;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
using FakeNewsBackend.Service.Interface;

namespace FakeNewsBackend.Service;

public class SitemapService : ISitemapService
{
    public void Add(Sitemap sim)
    {
        using (var context = new SitemapContext())
        {
            foreach (var sitemapItem in sim.links)
            {
                context.SitemapItems.Add(sitemapItem);
            }
            context.SaveChanges();
        }
    }

    public void Update(Sitemap sim)
    {
        using (var context = new SitemapContext())
        {
            foreach (var sitemapItem in sim.links)
            {
                context.SitemapItems.Update(sitemapItem);
            }
            context.SaveChanges();
        }
    }
    
    public void Delete(Sitemap sim)
    {
        using (var context = new SitemapContext())
        {
            foreach (var sitemapItem in sim.links)
            {
                context.SitemapItems.Remove(sitemapItem);
            }
            context.SaveChanges();
        }
    }
    
    public void SaveMultipleSitemaps(IEnumerable<SitemapItem> items)
    {
        using (var context = new SitemapContext())
        {
            foreach (var sitemapItem in items)
            {
                if(context.SitemapItems.ToList().Contains(sitemapItem)) 
                    continue;
                context.SitemapItems.Add(sitemapItem);
            }
            
            context.SaveChanges();
        }
    }

    public Sitemap GetSitemapByWebsiteId(int id)
    {
        using (var context = new SitemapContext())
        {
            var result = context.SitemapItems.Where(item => item.websiteId == id).ToList();

            return new Sitemap() { WebsiteId = id, links = result };
        }
    }

    public void UpdateItem(SitemapItem item)
    {
        using (var context = new SitemapContext())
        {
            context.SitemapItems.Update(item);
            context.SaveChanges();
        }
    }

    public bool ItemsExistsWithId(int websiteId)
    {
        using (var context = new SitemapContext())
        {
            return context.SitemapItems.ToList().Any(i => i.websiteId == websiteId);
        }
    }
    
    public List<UrlItemDTO> GetSitmapItemDtos(int websiteId)
    {
        using (var context = new SitemapContext())
        {
            var result = context.SitemapItems.ToList()
                .Where(i => i.websiteId == websiteId);
            return result.Select(i => new UrlItemDTO() { lastmod = i.date, url = i.url })
                .ToList();
        }
    }

    public void SaveGenerateProgress(SitemapGenerateProgress prog)
    {
        using (var context = new SitemapContext())
        {
            if (ProgressExists(prog.WebsiteId))
                context.GenerateProgress.Update(prog);
            else
                context.GenerateProgress.Add(prog);
            context.SaveChanges();
        }
    }
    public void DeleteGenerateProgress(SitemapGenerateProgress prog)
    {
        using (var context = new SitemapContext())
        {
            if(ProgressExists(prog.WebsiteId))
                context.GenerateProgress.Remove(prog);
            context.SaveChanges();
        }
    }

    public bool ProgressExists(int websiteId)
    {
        using (var context = new SitemapContext())
        {  
            return context.GenerateProgress
                .Any(pr => pr.WebsiteId == websiteId);
        }
    }

    public SitemapGenerateProgress GetProgress(int websiteId)
    {
        using (var context = new SitemapContext())
        {
            Console.WriteLine(websiteId);
            if(context.GenerateProgress != null && 
               context.GenerateProgress.Any(pr => pr.WebsiteId == websiteId ))
                return context.GenerateProgress
                    .First(pr => pr.WebsiteId == websiteId);
            return null;
        }
    }
}