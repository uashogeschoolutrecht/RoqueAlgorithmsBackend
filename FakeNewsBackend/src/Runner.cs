using System.Collections.Concurrent;
using FakeNewsBackend.Common;
using FakeNewsBackend.Common.debug;
using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Controller;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Parser;
using FakeNewsBackend.Service;
using NLog;

namespace FakeNewsBackend
{
    /// <summary>
    /// Controls the flow of the algorithm
    /// </summary>
    public class Runner
    {
        #region private fields

        #region controllers

        private WebController _webController;
        private HttpController _httpController;
        private ISearchController _searchController;
        private SimilarityController _similarityController;
        private SiteMapController _siteMapController;
        private RobotsController _robotsController;

        #endregion
        
        #region services

        private WebsiteService _webSiteService;
        private SimilarityService _similarityService;
        private SitemapService _sitemapService;
        private RobotRulesService _robotRulesService;

        #endregion


        private Random random;
        private string similarityUrl;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const int minimumArticlelength = 100;
        private const int maximumArticlelength = 40000;
        private string option;

        #endregion
        
        public void Setup()
        {
            random = new();
            
            _webSiteService = new();
            _similarityService = new();
            _sitemapService = new ();
            _robotRulesService = new();
            
            
            similarityUrl = "http://145.38.188.62:5000";
            
            
            _httpController = new(new HttpClient(), 75);
            _searchController = new BingSearchController(_httpController);
            _webController = new (_httpController, random, _webSiteService);
            _siteMapController = new (_httpController, random, _sitemapService);
            _robotsController = new (_httpController, _robotRulesService);
            _similarityController = new (_httpController, _similarityService, similarityUrl);
        }
        public async Task Run()
        {
            // await SetSimilaritiesInJson();
            // return;
            if (!_webController.HasWebsites())
            {
                _logger.Info("Seeding the database.");
                await SeedDatabase();
            }
            //var t1 = Task.Run(async () => await SetUpWebsites());
            //var t2 = Task.Run(async () => await ThroughWebsites());

            //await Task.WhenAll( t2);
            CheckSimilarities();
            Console.WriteLine("Done");
        }

        public async Task<int> SetUpWebsites()
        {
            ThreadPool.SetMaxThreads(5,5);
            var runningWebsites = new ConcurrentDictionary<Website, bool>();
            var i = 0;
            while (i < 5)
            {
                if (!_webController.HasWebsiteToSetup())
                {
                    Console.WriteLine("Database has no new website, trying again.");
                    Thread.Sleep(3000);
                    continue;
                }
                var website = _webController.GetWebsiteToSetup();
                if(runningWebsites.ContainsKey(website))
                    continue;
                runningWebsites.TryAdd(website, true);
                ThreadPool.QueueUserWorkItem(async (o) =>
                {
                    await SetupWebsite(website.Url);
                    runningWebsites.TryRemove(
                        new KeyValuePair<Website, bool>(website, true));
                });
                i++;
            }
            return 0;
        }

        public async Task<int> ThroughWebsites()
        {
            loop:
            while (_webController.HasNextWebsite())
            {
                Console.WriteLine("Walking thread working");
                var website = _webController.GetUncompletedWebsite();
                var completeWebsite = await GetInfo(website);

                await GoThroughWebSite(completeWebsite);
            }
            await SetUpWebsites();
            goto loop;
            return 0;
        }

        public async Task GoThroughWebSite(Website website)
        {
            _logger.Trace("Going through: {Website}",website.Name);
            var result =
                website.Sitemap.links.Where(i => !i.scraped && website.IsLinkAllowed(i.url))
                    .ToList();
            var lengthToloop = result.Count > 25 ? 25 : result.Count;
            var handling = new List<Task<bool>>();
            for (var i = 0; i < lengthToloop; i++)
            {
                handling.Add(HandleArticle(result[i]));
                await Task.Delay(250);
            }
            Console.WriteLine("Waiting for all articles to be handled");
            var doneHandling = await Task.WhenAll(handling);
            for(var i = 0; i < doneHandling.Count(); i++)
            {
                if (!doneHandling[i])
                    continue;
                website.NumberOfArticlesScraped++;
                website.Progress.NumberOfLinkInSiteMap++;
                website.Progress.CurrentlyWorkingOn = result[i].url;
                result[i].scraped = true;
                _webController.UpdateWebsite(website);
                _siteMapController.UpdateSitemapItem(result[i]);
            }

            if (website.NumberOfArticles == website.NumberOfArticlesScraped)
            {
                _logger.Trace("{Website} is now done", website.Name);
                website.Progress.IsDone = true;
            }
            Console.WriteLine("Done waiting");    
            _webController.UpdateWebsite(website);
            await Task.Delay(500);
        }

        public async Task SetupWebsite(string link)
        {
            _logger.Trace($"Setting up Website: {link}");
            var website = await _webController.GetWebSite(link);
            website = await GetInfo(website);
            website.NumberOfArticles = website.Sitemap.getTotalLinks();
            
            website.HasSitemap = true;
            website.HasRules = true;
            _webController.UpdateWebsite(website);
            
            _logger.Trace($"Website: {link} set up");
        }

       
        
        public async Task SeedDatabase()
        {
            var whitelist = Config.GetDataBaseSeed("whitelist");
            var seed = Config.GetDataBaseSeed("seed");
            await _webController.SeedDatabase(whitelist, seed);
        }

        public async Task<Website> GetInfo(Website website)
        {
            website.Rules = await _robotsController.GetRobotsRules(website);
            website.Sitemap = await _siteMapController.GetSitemapOfWebsite(website);

            return website;
        }
        
        public async Task<bool> HandleArticle(SitemapItem item, int depth = 0)
        {
            try
            {
                Console.WriteLine($"Working with: {item.url}");

                var originalPage = await _webController.RequestWebPage(item.url, item.date);
                if (originalPage == null || originalPage.MainContent == "Not Found" ||
                    originalPage.MainContent.Length < minimumArticlelength ||
                    originalPage.MainContent.Length > maximumArticlelength)
                {
                    _logger.Warn($"Invalid article was found for: {item.url}");
                    return true;
                }

                Console.WriteLine(originalPage.DatePosted.ToString());
                if (!_siteMapController.IsRecentEnough(originalPage.DatePosted, originalPage.Url))
                    return true;

                var search = $"{originalPage.Title} {WebParser.GetFirstSentence(originalPage.MainContent)}";
                Console.WriteLine($"searching: {search}");
                var links = await _searchController.SearchTitle(
                    search, originalPage.HostName);
                if (links == null)
                    return true;
                Console.WriteLine($"Requesting {links.Count()} articles");

                var foundLinks = await _webController.RequestMultipleArticles(
                    links.ToList(), false);
                Console.WriteLine($"Getting similarity scores for article {item.url},\n{foundLinks.Count} times");
                Console.WriteLine($"Article length: {originalPage.MainContent.Length}");
                
                var filtered = new List<WebPage>();
                foreach (var page in foundLinks)
                {
                    if (page == null)
                        continue;
                    if (_similarityController.Exists(originalPage, page))
                        continue;
                    if (page.MainContent.Length < minimumArticlelength ||
                        page.MainContent.Length > maximumArticlelength)
                        continue;
                    filtered.Add(page);
                }
                var sims = _similarityController.GetSimilarityScores(originalPage, filtered);
                while (!sims.IsCompleted)
                {
                    await Task.Delay(25);
                }

                foreach (var sim in sims.Result)
                    await HandleSimilarity(sim, item);
                
                return true;
            }
            catch (JsonElementParseException e)
            {
                _logger.Error(e, "Json results failed, article: {Article}", item.url);
            }
            catch (JsonElementNotFoundException e)
            {
                _logger.Error(e, "Search results where not there, article: {Article}", item.url);
            }
            catch (GoogleExhaustedException e)
            {
                _logger.Warn("The google search API has reached the limit");
                await Task.Delay(GoogleSearchController.GOOGLE_RETRY_DELAY);
                return false;
            }
            catch (SimilarityException e)
            {
                _logger.Fatal(e, "Similarty model is unavailible");
                
                if (depth <= 1)
                {
                    Thread.Sleep(SimilarityController.WaitLimitToRetry);
                    return await HandleArticle(item, depth++);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e,"Handling failed, article: {Article}", item.url);
            }
            return true;
        }

        public async Task<int> HandleSimilarity(Similarity sim, SitemapItem origItem)
        {
            if (!_similarityController.ShouldSave(sim))
            {
                Console.WriteLine("Should not save");
                return 0;
            }
                
            
            var foundurl = (sim.UrlToOriginalArticle == origItem.url)
                ? sim.UrlToFoundArticle
                : sim.UrlToOriginalArticle;

            var foundWebsite = new Uri(foundurl);
            var url = UrlUtils.SanetizeUrl(
                foundWebsite.OriginalString
                    .Replace(foundWebsite.PathAndQuery, ""));

            var areSimilar = _similarityController.AreSimilar(sim);
            var newWebsite =  areSimilar ? 
                _webController.GetWebSite(url) : 
                _webController.GetWhitelistedWebsite(url);
            
            if(areSimilar)
                _logger.Info("New website found: {Website}", url);
            Console.WriteLine($"Sim handeling : {sim.SimilarityScore}");
            while (!newWebsite.IsCompleted)
            {
                await Task.Delay(25);
            }
            var website = newWebsite.Result;
            if(website == null)
                return 0;
            if (areSimilar && website.IsWhitelisted && !website.ShouldNotBeScraped)
            {
                website.IsWhitelisted = false;
                _webController.UpdateWebsite(website);
            }
            
            if (sim.UrlToOriginalArticle == origItem.url)
            {
                sim.FoundWebsiteId = website.Id;
                sim.OriginalWebsiteId = origItem.websiteId;
            }
            else if (sim.UrlToFoundArticle == origItem.url)
            {
                sim.OriginalWebsiteId = website.Id;
                sim.FoundWebsiteId = origItem.websiteId;
            }

            _similarityController.Save(sim);
            return 0;
        }

        public void CheckSimilarities()
        {
            Console.WriteLine("checking sims");
            var similaritiesToCheck = _similarityController.GetSimilaritiesWithUncertainUrls();
            var i = 0;
            foreach( var sim in similaritiesToCheck)
            {
                lock (sim)
                {
                    i++;
                    if (i > 3)
                        break;
                    Console.WriteLine(sim.ToString());
                    var originalSim = sim;
                    var hasUpdatedDate = false;
                    var onlyFoundMonth = false;
                    if (sim.FoundPostDate == DateTime.MinValue && UrlUtils.UrlHasTotalDate(sim.UrlToFoundArticle))
                    {
                        sim.FoundPostDate = UrlUtils.GetDateOutOfUrl(sim.UrlToFoundArticle);
                        hasUpdatedDate = true;
                    }
                    else if (sim.FoundPostDate == DateTime.MinValue && UrlUtils.UrlHasMonth(sim.UrlToFoundArticle))
                    {
                        sim.FoundPostDate = UrlUtils.GetMonthOutOfUrl(sim.UrlToFoundArticle);
                        hasUpdatedDate = true;
                        onlyFoundMonth = true;
                    }
                    if (sim.OriginalPostDate == DateTime.MinValue && UrlUtils.UrlHasTotalDate(sim.UrlToOriginalArticle))
                    {
                        sim.OriginalPostDate = UrlUtils.GetDateOutOfUrl(sim.UrlToOriginalArticle);
                        hasUpdatedDate = true;
                    }
                    else if (sim.FoundPostDate == DateTime.MinValue && UrlUtils.UrlHasMonth(sim.UrlToFoundArticle))
                    {
                        sim.FoundPostDate = UrlUtils.GetMonthOutOfUrl(sim.UrlToFoundArticle);
                        hasUpdatedDate = true;
                        onlyFoundMonth = true;
                    }
                    Console.WriteLine(sim.ToString());
                    if (hasUpdatedDate && _similarityController.ShouldSwap(sim, onlyFoundMonth ) ) 
                    {
                        sim.swap();
                        _similarityController.UpdateSimilarityAfterSwap(originalSim, sim);
                        Console.WriteLine(sim.ToString());

                    }else if (hasUpdatedDate)
                    {
                        _similarityController.UpdateSimilarity(sim);
                    }
                    Console.WriteLine("---");
                }

            }
        }
    }
        
}