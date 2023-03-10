using System.Globalization;
using System.Text;
using FakeNewsBackend.Common;
using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Common.Extensions;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
using FakeNewsBackend.Service.Interface;
using Newtonsoft.Json;
using NLog;

namespace FakeNewsBackend.Controller;

public class SimilarityController
{
    private readonly HttpController _httpController;
    private readonly ISimilarityService _similarityService;
    private readonly string SimilarityUrl;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    private const Single SimilarityThreshold = 0.7F;
    private const Single SaveThreshold = 0.3F;

    public const int WaitLimitToRetry = 15000; 

    public SimilarityController(HttpController httpController, ISimilarityService similarityService) :
        this(httpController, similarityService, "http://127.0.0.1:5000") { }

    public SimilarityController(HttpController httpController, 
        ISimilarityService similarityService, string similarityUrl)
    {
        _httpController = httpController;
        _similarityService = similarityService;
        SimilarityUrl = similarityUrl + "/similarity";
    }

    /// <summary>
    /// Get the similarity score of two articles.
    /// </summary>
    /// <param name="originalWebPage">One of the articles to use.</param>
    /// <param name="foundWebPage">One of the articles to use.</param>
    /// <returns>A <see cref="Task"/> containing A <see cref="Similarity"/> object.</returns>
    /// <exception cref="JsonElementParseException">If something is wrong with Json repsonse.</exception>
    public async Task<Similarity> GetSimilarityScore(WebPage originalWebPage, WebPage foundWebPage)
    {
        IDictionary<string, string> body = new Dictionary<string, string>
        {
            { "original_article", Encode(originalWebPage.MainContent) },
            { "found_article", Encode(foundWebPage.MainContent) },
            { "original_language", Mapper.LanguageToString(originalWebPage.Language) },
            { "found_language", Mapper.LanguageToString(foundWebPage.Language) },
            { "type", "cosine" }
        };
        var request = await _httpController.MakePostRequest(SimilarityUrl, body);
        if (request.content == "Unavailable")
            throw new SimilarityException($"The similarity model is unavailible: {SimilarityUrl}");
    
        Console.WriteLine(request.content);
        if (!JsonDocumentExtension.TryParse(request.content, out var document))
            throw new JsonElementParseException("Similarity Json is wrong");
        if (!document.RootElement.TryGetProperty("similarity", out var sim))
            throw new JsonElementParseException("Could not find Similarity");

        return GenerateSimilarity(originalWebPage, foundWebPage, sim.GetSingle());
    }

    /// <summary>
    /// Get the similarity score of multiple articles.
    /// </summary>
    /// <param name="orginalPage">The orginal article</param>
    /// <param name="foundPages">An <see cref="IEnumerable{T}"/> with all the found articles</param>
    /// <returns>A <see cref="Task"/> containing A <see cref="Similarity"/> object.</returns>
    /// <exception cref="JsonElementParseException">If something is wrong with Json repsonse.</exception>
    public async Task<IEnumerable<Similarity>> GetSimilarityScores(WebPage orginalPage, IEnumerable<WebPage> foundPages)
    {
        var foundWebPages = foundPages.Select(
            page => new SimilarityDTO()
            {
                article = page.MainContent,
                language = Mapper.LanguageToString(page.Language),
                url = page.Url
            });
        var Json = JsonConvert.SerializeObject(foundWebPages);
        IDictionary<string, string> body = new Dictionary<string, string>
        {
            { "original_article", Encode(orginalPage.MainContent) },
            { "original_language", Mapper.LanguageToString(orginalPage.Language) },
            { "found_articles", Json},
            { "type", "cosine" }
        };

        var request = await _httpController.MakePostRequest(SimilarityUrl, body);
        if (request.content == "Unavailable")
            throw new SimilarityException($"The similarity model is unavailible: {SimilarityUrl}");

        if (!JsonDocumentExtension.TryParse(request.content, out var document))
            throw new JsonElementParseException("Similarity Json is wrong");
        if (!document.RootElement.TryGetProperty("similarities", out var sim))
            throw new JsonElementParseException("Could not find Similarity");
        var sims = sim.EnumerateArray()
            .Select(el => GenerateSimilarity(orginalPage, 
                foundPages.First(p => p.Url.Equals(el.GetProperty("url").GetString())), 
                Single.Parse(el.GetProperty("sim").GetString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)
                ));
        return sims;
    }
    /// <summary>
    /// Checks if Similarity already exists.
    /// </summary>
    /// <param name="originalPage">One of the articles to check.</param>
    /// <param name="foundPage">One of the articles to check.</param>
    /// <returns>Whether the Similarity between the params exists.</returns>
    public bool Exists(WebPage originalPage, WebPage foundPage)
    {
        return _similarityService.Exists(originalPage, foundPage);
    }

    public IEnumerable<Similarity> GetSimilaritiesFromTwoWebsites(Website website1, Website website2)
    {
        return _similarityService.GetSimilaritiesByWebsitesIdInOrder(website1.Id, website2.Id);
    }

    public IEnumerable<Similarity> GetSimilaritiesWithUncertainUrls()
    {
        return _similarityService.GetSimilaritiesWithUncertainUrls();
    }

    /// <summary>
    /// Save a <see cref="Similarity"/> object in the database.
    /// </summary>
    /// <param name="sim"><see cref="Similarity"/> to save.</param>
    public void Save(Similarity sim)
    {
        _similarityService.Add(sim);
    }

    public IEnumerable<Similarity> GetAllSimilarities()
    {
        return _similarityService.GetAll();
    }

    /// <summary>
    /// Checks if similarity score is above the threshold.
    /// </summary>
    /// <param name="sim"><see cref="Similarity"/> to check</param>
    /// <returns>Whether the similarity score is higher than the <see cref="SimilarityThreshold"/></returns>
    public bool AreSimilar(Similarity sim)
    {
        return sim.SimilarityScore >= SimilarityThreshold;
    }

    public bool ShouldSave(Similarity sim)
    {
        return sim.SimilarityScore >= SaveThreshold;
    }

    private string Encode(string str)
    {
        return Encoding.UTF8.GetString(
            Encoding.UTF8.GetBytes(str));
    }

    public Similarity GenerateSimilarity(WebPage originalWebPage, WebPage foundWebPage, Single sim)
    {
        Tuple<WebPage, WebPage> order;
        if ((originalWebPage.DatePosted.CompareTo(foundWebPage.DatePosted) <= 0 &&
             originalWebPage.DatePosted != DateTime.MinValue) || 
            foundWebPage.DatePosted == DateTime.MinValue)
            order = new Tuple<WebPage, WebPage>(originalWebPage, foundWebPage);
        else
            order = new Tuple<WebPage, WebPage>(foundWebPage, originalWebPage);
        
        return new Similarity()
        {
            OriginalWebsiteId = order.Item1.WebsiteId,
            FoundWebsiteId = order.Item2.WebsiteId,
            OriginalLanguage = order.Item1.Language,
            FoundLanguage = order.Item2.Language,
            SimilarityScore = sim,
            UrlToOriginalArticle = order.Item1.Url,
            UrlToFoundArticle = order.Item2.Url,
            OriginalPostDate = order.Item1.DatePosted,
            FoundPostDate = order.Item2.DatePosted,
        };
    }
    public void UpdateSimilarity(Similarity sim)
    {
        _similarityService.Update(sim);
    }

    public bool ShouldSwap(Similarity sim, bool onlyUpdatedWithMonth, bool onlyFoundYear)
    {
        if(onlyUpdatedWithMonth)
            return sim.OriginalPostDate.Month > sim.FoundPostDate.Month;
        if(onlyFoundYear)
            return sim.OriginalPostDate.Year > sim.FoundPostDate.Year;
        return sim.OriginalPostDate.CompareTo(sim.FoundPostDate) > 0;
    }
    
    public void CheckSimilarities()
    {
        var similaritiesToCheck = GetSimilaritiesWithUncertainUrls().Distinct();
        Console.WriteLine(similaritiesToCheck.Count());
        var updatedSimilarities = new List<(SimilarityIds, Similarity, bool)>();
        foreach( var sim in similaritiesToCheck)
        {
            var originalSim =  new SimilarityIds() 
            {
                originalId = sim.OriginalWebsiteId,
                foundId = sim.FoundWebsiteId,
                originalUrl = sim.UrlToOriginalArticle, 
                foundUrl = sim.UrlToFoundArticle  ,
                originalDate = sim.OriginalPostDate,
                foundDate = sim.FoundPostDate,
            };
            var hasUpdatedDate = false;
            var onlyFoundMonth = false;
            var hasSwapped = false;
            var onlyFoundYear = false;
            if(sim.FoundPostDate == DateTime.MinValue)
            {
                var (date, updated, month, year) = GetDateFromUrl(sim.UrlToFoundArticle, sim.FoundPostDate, 
                    hasUpdatedDate,onlyFoundMonth, onlyFoundYear);
                sim.FoundPostDate = date;
                hasUpdatedDate = updated;
                onlyFoundMonth = month;
                onlyFoundYear = year;
            }
            if (sim.OriginalPostDate == DateTime.MinValue)
            {
                var (date, updated, month, year) = GetDateFromUrl(sim.UrlToOriginalArticle, sim.OriginalPostDate,
                    hasUpdatedDate, onlyFoundMonth, onlyFoundYear);
                sim.OriginalPostDate = date;
                hasUpdatedDate = updated;
                onlyFoundMonth = month;
                onlyFoundYear = year;
            }
            if (!hasUpdatedDate)
                continue;
            
            if (ShouldSwap(sim, onlyFoundMonth , onlyFoundYear) )
            {
                sim.swap();
                hasSwapped= true;
            }
            updatedSimilarities.Add((originalSim,sim, hasSwapped));
            Console.WriteLine("---+++---");
        }
        
        Console.WriteLine("Updated similarities, now saving");
        _similarityService.UpdateSimilarityAfterSwap(updatedSimilarities);
    }

    public (DateTime, bool, bool, bool) GetDateFromUrl(string url, DateTime date,  bool hasUpdated, bool foundMonth, bool foundYear)
    {
        if (UrlUtils.UrlHasTotalDate(url))
        {
            date = UrlUtils.GetDateOutOfUrl(url);
            hasUpdated = true;
        }
        else if (UrlUtils.UrlHasMonth(url))
        {
            date = UrlUtils.GetMonthOutOfUrl(url);
            hasUpdated = true;
            foundMonth = true;
        }
        else if (UrlUtils.UrlHasDate(url))
        {
            date = UrlUtils.GetYearOutOfUrlAsDate(url);
            hasUpdated = true;
            foundYear = true;
        }
        return (date, hasUpdated, foundMonth, foundYear);
    }
}