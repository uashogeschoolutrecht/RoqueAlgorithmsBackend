using FakeNewsBackend.Context;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
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
                sim.OriginalPostDate == DateTime.MinValue).Distinct().ToHashSet();
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
    public bool SimilarityExists(Similarity similarity)
    {
        using (var context = new SimilarityContext())
        {
            return context.Similarities.Any(sim => sim.Equals(similarity));
        }
    }
    public Similarity FindSimilarity(int originalId, int foundId, string originalUrl, string foundUrl)
    {
        using (var context = new SimilarityContext())
        {
            return context.Similarities.First(sim => 
                sim.OriginalWebsiteId == originalId && 
                sim.FoundWebsiteId == foundId &&
                sim.UrlToOriginalArticle == originalUrl &&
                sim.UrlToFoundArticle == foundUrl);
        }
    }

    public Similarity GetSimilarityByKeys(SimilarityIds ids)
    {
        using (var context = new SimilarityContext())
        {
            return context.Similarities.First(sim =>
                sim.OriginalWebsiteId == ids.originalId &&
                sim.FoundWebsiteId == ids.foundId &&
                sim.UrlToOriginalArticle == ids.originalUrl &&
                sim.UrlToFoundArticle == ids.foundUrl);
        }
    }

    public void UpdateSimilarityAfterSwap(IEnumerable<(SimilarityIds similarityIds, Similarity updated, bool isUpdated)> similarities)
    {
        Console.WriteLine(similarities.Count());
        var totalSwapped = 0;
        using(var context = new SimilarityContext())
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach(var (original, updated, isUpdated) in similarities)
                    {
                        /*if (!isUpdated && original.originalUrl == updated.UrlToOriginalArticle && 
                            original.foundUrl == updated.UrlToFoundArticle && (
                            original.originalDate != updated.OriginalPostDate || original.foundDate != updated.FoundPostDate) &&
                            !context.ChangeTracker.Entries<Similarity>().Any(e => e.Entity == updated))
                        {
                            context.Entry(updated).Property("Timestamp").OriginalValue = updated.Timestamp;
                            context.Entry(updated).State = EntityState.Detached;
                            context.Similarities.Update(updated);
                            continue;
                        }
                        */
                        try
                        {
                            var dbsim = GetSimilarityByKeys(original);
                            context.Entry(dbsim).Property("Timestamp").OriginalValue = dbsim.Timestamp;
                            context.Entry(dbsim).Reload();
                            context.Similarities.Remove(dbsim);
                        
                            if (SimilarityExists(updated))
                            {
                                context.Similarities.Update(updated);
                                continue;
                            }
                            context.Similarities.Add(updated);
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        
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
                catch (DbUpdateException e) 
                {
                    Console.WriteLine(e);
                    transaction.Rollback();
                }
            }
        }
    }
}
