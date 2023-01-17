using FakeNewsBackend.Context;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace FakeNewsBackend.Service;
public class SimilarityService : ISimilarityService
{
    public SimilarityService() { }
    
    public void Add(Similarity sim)
    {
        using (var context = new SimilarityContext())
        {
            if (context.Similarities.Contains(sim))
                context.Similarities.Update(sim);
            else
                context.Similarities.Add(sim);
            context.SaveChanges();
        }
    }

    public void Update(Similarity sim)
    {
        using (var context = new SimilarityContext())
        {
            context.Similarities.Update(sim);
            context.SaveChanges();
        }
    }
    public void Delete(Similarity sim)
    {
        using (var context = new SimilarityContext())
        {
            context.Similarities.Remove(sim);
            context.SaveChanges();
        }
    }
    
    public Similarity GetSimilarityByLinks(string firstLink, string secondLink)
    {
        using (var context = new SimilarityContext())
        {
            context.Database.OpenConnection();
            var foundSimilarity = context.Similarities.FirstOrDefault(sim =>
                (sim.UrlToOriginalArticle == firstLink && sim.UrlToFoundArticle == secondLink) ||
                (sim.UrlToOriginalArticle == secondLink && sim.UrlToFoundArticle == firstLink)
            );
            return foundSimilarity;
        }
    }
    public IEnumerable<Similarity> GetSimilaritiesWithUncertainUrls() 
    { 
        using(var context = new SimilarityContext())
        {
            return context.Similarities.ToArray().Where(sim => 
                sim.FoundPostDate == DateTime.MinValue ||
                sim.OriginalPostDate == DateTime.MinValue);
        }
    }

    public IEnumerable<Similarity> GetSimilaritiesByWebsitesId(int webSite1Id, int webSite2Id)
    {
        using (var context = new SimilarityContext())
        {
            var foundSimilarity = context.Similarities.Where(sim =>
                (sim.OriginalWebsiteId == webSite1Id && sim.FoundWebsiteId == webSite2Id) ||
                (sim.OriginalWebsiteId == webSite2Id && sim.FoundWebsiteId == webSite1Id)
            );
            return foundSimilarity.ToList();
        }
    }
    public IEnumerable<Similarity> GetSimilaritiesByWebsitesIdInOrder(int webSite1Id, int webSite2Id)
    {
        using (var context = new SimilarityContext())
        {
            var foundSimilarity = context.Similarities.Where(sim =>
                (sim.OriginalWebsiteId == webSite1Id && sim.FoundWebsiteId == webSite2Id)
            );
            return foundSimilarity.ToList();
        }
    }
    public IEnumerable<Similarity> GetSimilaritiesByWebsiteId(int id)
    {
        using (var context = new SimilarityContext())
        {
            var foundSimilarity = context.Similarities.Where(sim =>
                sim.OriginalWebsiteId == id || sim.FoundWebsiteId == id
            );
            return foundSimilarity;
        }
    }

    public IEnumerable<Similarity> GetAll()
    {
        using (var context = new SimilarityContext())
        {
            return context.Similarities.ToList();
        }
    }

    public bool Exists(WebPage originalPage, WebPage foundPage)
    {
        if (originalPage == null || foundPage == null)
        {
            return false;
        }
        using (var context = new SimilarityContext())
        {
            var result = context.Similarities.FirstOrDefault(sim =>
                (sim.FoundWebsiteId == foundPage.WebsiteId && sim.OriginalWebsiteId == originalPage.WebsiteId &&
                 sim.UrlToFoundArticle == foundPage.Url && sim.UrlToOriginalArticle == originalPage.Url) ||
                    (sim.FoundWebsiteId == originalPage.WebsiteId && sim.OriginalWebsiteId == foundPage.WebsiteId &&
                     sim.UrlToFoundArticle == originalPage.Url && sim.UrlToOriginalArticle == foundPage.Url)
            );
            return result != null;
        }
    }

    public void UpdateSimilarityAfterSwap(Similarity oldSim, Similarity newSim)
    {
        using(var context = new SimilarityContext())
        {
            context.Similarities.Attach(oldSim);
            context.Similarities.Remove(oldSim);
            context.SaveChanges();
            context.Similarities.Add(newSim);
            context.SaveChanges();
        }
    }
}
