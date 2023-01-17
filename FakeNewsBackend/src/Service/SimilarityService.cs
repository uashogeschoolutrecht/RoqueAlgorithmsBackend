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
            return context.Similarities.Where(sim => 
                sim.FoundPostDate == DateTime.MinValue ||
                sim.OriginalPostDate == DateTime.MinValue).ToList();
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

    public void UpdateSimilarityAfterSwap(IEnumerable<((string, string), Similarity updated)> similarities)
    {
        using(var context = new SimilarityContext())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach(var (original, updated) in similarities)
                    {
                        if (original.Item1 == updated.UrlToOriginalArticle)
                        {
                            context.Similarities.Update(updated);
                            continue;
                        }

                        var dbsim = GetSimilarityByLinks(original.Item1, original.Item2);
                        context.Similarities.Remove(dbsim);
                        context.Similarities.Add(updated);
                    }
                    context.SaveChanges();
                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    Console.WriteLine(e);
                    transaction.Rollback();
                    var entry = e.Entries.Single();
                    var clientValues = (Similarity)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        Console.WriteLine("Unable to save changes. The similarity was deleted by another user.");
                    }
                }
            }
        }
    }
}
