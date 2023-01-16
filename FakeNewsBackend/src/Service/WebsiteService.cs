using FakeNewsBackend.Common.Extensions;
using FakeNewsBackend.Context;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Service.Interface;

namespace FakeNewsBackend.Service;
public class WebsiteService : IWebsiteService
{
    public WebsiteService() { }

    public void Add(Website web)
    {
        using (var context = new WebsiteContext())
        {
            context.Websites.Add(web);
            context.SaveChanges();
        }
    }
    
    public void Update(Website web)
    {
        using (var context = new WebsiteContext())
        {
            context.Websites.Update(web);
            context.SaveChanges();
        }
    }
    public void Delete(Website web)
    {
        using (var context = new WebsiteContext())
        {
            context.Websites.Remove(web);
            context.SaveChanges();
        }
    }
    
    public void SaveProgress(WebsiteProgress progress)
    {
        using (var context = new WebsiteContext())
        {
            context.Progress.Add(progress);
            context.SaveChanges();
        }
    }
    public void UpdateProgress(WebsiteProgress progress)
    {
        using (var context = new WebsiteContext())
        {
            context.Progress.Update(progress);
            context.SaveChanges();
        }
    }
    public bool WebsiteExistsWithUrl(string web)
    {
        using (var context = new WebsiteContext())
        {
            return context.Websites.Any(site => site.Url == web);
        }
    }
    
    public Website GetWebSiteByUrl(string url)
    {
        using (var context = new WebsiteContext())
        {
            return context.Websites.ToList()
                .FirstOrDefault(site => site.Url == url, null);
        }
    }
    public WebsiteProgress GetProgressById(int id)
    {
        using (var context = new WebsiteContext())
        {
            return context.Progress.ToList()
                .FirstOrDefault(prog => prog.WebsiteId == id, null);
        }
    }

    public IEnumerable<Website> GetAll()
    {
        using (var context = new WebsiteContext())
        {
            return context.Websites.ToList();
        }
    }

    public Website GetWebsiteToSetup()
    {
        using (var context = new WebsiteContext())
        {
            return context.Websites.ToList()
                .Where(website => !website.HasRules && 
                                           !website.HasSitemap && 
                                           !website.IsWhitelisted && 
                                           !website.ShouldNotBeScraped)
                .Sample();
        }
    }
    public bool HasWebsiteToSetup()
    {
        using (var context = new WebsiteContext())
        {
            return context.Websites
                .Any(website => !website.HasRules && 
                                !website.HasSitemap && 
                                !website.IsWhitelisted &&
                                !website.ShouldNotBeScraped);
        }
    }

    public bool HasNextWebsite()
    {
        using (var context = new WebsiteContext())
        {
            return context.Websites.Any(website =>
                website.HasRules &&
                    website.HasSitemap &&
                    !website.IsWhitelisted &&
                    !website.ShouldNotBeScraped &&
                    website.NumberOfArticles != website.NumberOfArticlesScraped
            );
        }
    }

    public bool HasWebsites()
    {
        using (var context = new WebsiteContext())
        {
            return context.Websites.Any();
        }
    }
    public Website GetUncompletedWebsite()
    {
        using (var context = new WebsiteContext())
        {
            var setupwebsites = context.Websites.ToList()
                .Where(website => website.HasRules && 
                                  website.HasSitemap && 
                                  !website.IsWhitelisted &&
                                  !website.ShouldNotBeScraped)
                .Select(website => website.Id);
            
            var prog = context.Progress.ToList()
                .Where(prog => !prog.IsDone && setupwebsites.Contains(prog.WebsiteId))
                .Sample();
            
            if (prog == null) return null;
            return context.Websites.ToList()
                .FirstOrDefault(website => website.Id == prog.WebsiteId, null );
        }
    }
    
}
