using FakeNewsBackend.Builder;
using FakeNewsBackend.Common;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
using FakeNewsBackend.Parser;
using FakeNewsBackend.Service.Interface;
using NLog;

namespace FakeNewsBackend.Controller
{
    public class WebController
    {
        private readonly HttpController _httpController;
        private readonly Random _random;
        private readonly IWebsiteService _siteService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public WebController(HttpController httpController, Random random, IWebsiteService _service)
        { 
            _httpController = httpController;
            _random = random;
            _siteService = _service;
        }
        public WebController(HttpController httpController, IWebsiteService _websiteService) :
        this(httpController, new Random(), _websiteService){}

        /// <summary>
        /// Request Webpage from given Url.
        /// </summary>
        /// <param name="link">Url of webpage.</param>
        /// <param name="possibleDate">Date to fall back on if no date was found on the page. (can be null)</param>
        /// <returns>A <see cref="Task"/> containing the <see cref="WebPage"/>.</returns>
        public async Task<WebPage> RequestWebPage(string link, DateTime possibleDate = default)
        {
            var requestResult = await _httpController.MakeGetRequest(link.Replace("\"",""));
            var webPage = GetWebPage(requestResult.content, requestResult.Response.RequestMessage?.RequestUri, link);
            
            if(possibleDate != default && webPage.DatePosted == null)
                webPage.DatePosted = possibleDate;
            return webPage;
        }

        /// <summary>
        /// Request multiple articles from given list.
        /// </summary>
        /// <param name="items"><see cref="List{T}"/> of <see cref="UrlItemDTO"/>
        /// objects to request the article from.</param>
        /// <param name="isFromSameHost">If the articles are from the same host.</param>
        /// <returns>A <see cref="Task"/> containing a <see cref="List{T}"/>
        /// with all the articles as <see cref="WebPage"/>.</returns>
        public async Task<List<WebPage>> RequestMultipleArticles(List<UrlItemDTO> items, bool isFromSameHost)
        {
            List<Task<WebPage>> webPages = new();
            for (var i = 0; i < items.Count; i++)
            {
                if(BlackList.LinkIsInBlackList(items[i].url))
                    continue;
                if(isFromSameHost)
                    Thread.Sleep(_random.Next(1000,3000));
                Console.WriteLine("Making article Request: " + items[i].url);
                try
                {
                    webPages.Add(RequestWebPage(items[i].url, items[i].lastmod));
                }
                catch (Exception e)
                {
                    _logger.Error(e,"Something went wrong with requesting: {Link}", items[i].url);
                    Console.WriteLine(e);
                }
            }
            var pages = await Task.WhenAll(webPages);
            return pages.ToList() ;
        }

        /// <summary>
        /// Get the website from a given link.
        /// </summary>
        /// <param name="link">Url of the website.</param>
        /// <returns>A <see cref="Task"/> containing a <see cref="Website"/>.</returns>
        public async Task<Website> GetWebSite(string link)
        {
            if (WebsiteExistsWithUrl(link))
                return _siteService.GetWebSiteByUrl(link);
            
            var result = await _httpController.MakeGetRequest(link);
            if (!((int)result.Response.StatusCode >= 200 && (int)result.Response.StatusCode < 300))
                return null;
            var parser = new WebParser(result.content);
            
            var newWebsite = new Website() {
                Name = parser.GetWebsiteName(new Uri(link)),
                Url = UrlUtils.SanetizeUrl(link),
                Rules = new RobotRules(),
                HasSitemap = false,
                HasRules = false,
                IsWhitelisted = false,
                ShouldNotBeScraped = false
            };
            _siteService.Add(newWebsite);
            newWebsite = _siteService.GetWebSiteByUrl(newWebsite.Url);
            var progress = new WebsiteProgress()
            {
                WebsiteId = newWebsite.Id,
                IsDone = false,
                CurrentlyWorkingOn = "Not yet started"
            };
            _siteService.SaveProgress(progress);
            newWebsite.Progress = progress;
            
            _siteService.Update(newWebsite);
            return newWebsite;
        }

        /// <summary>
        /// Retrieve uncompleted <see cref="Website"/> which are setup from database.
        /// </summary>
        /// <returns>A uncompleted <see cref="Website"/>.</returns>
        public Website GetUncompletedWebsite()
        {
            return _siteService.GetUncompletedWebsite();
        }

        public async Task<Website> GetWhitelistedWebsite(string url)
        {
            if(_siteService.WebsiteExistsWithUrl(url))
                return _siteService.GetWebSiteByUrl(url);
            
            var website = await GetWebSite(url);
            website.IsWhitelisted = true;
            _siteService.Update(website);
            return website;
        }

        /// <summary>
        /// Get a website from the database that is not set up.
        /// </summary>
        /// <returns>A <see cref="Website"/> to setup.</returns>
        public Website GetWebsiteToSetup()
        {
            return _siteService.GetWebsiteToSetup();
        }

        /// <summary>
        /// Checks if the database contains a <see cref="Website"/> which is not set up.
        /// </summary>
        /// <returns>A <see cref="bool"/> whether the database contains a <see cref="Website"/>
        /// which is not set up.</returns>
        public bool HasWebsiteToSetup()
        {
            return _siteService.HasWebsiteToSetup();
        }

        /// <summary>
        /// Checks if the database contains a <see cref="Website"/> which is not completed.
        /// </summary>
        /// <returns>A <see cref="bool"/> whether the database contains an uncompleted <see cref="Website"/>.</returns>
        public bool HasNextWebsite()
        {
            return _siteService.HasNextWebsite();
        }

        /// <summary>
        /// Checks if the database contains websites.
        /// </summary>
        /// <returns>A <see cref="bool"/> whether the database contains a <see cref="Website"/>.</returns>
        public bool HasWebsites()
        {
            return _siteService.HasWebsites();
        }
        
        /// <summary>
        /// Updates <see cref="Website"/> in database.
        /// </summary>
        /// <param name="webSite"><see cref="Website"/> to update.</param>
        public void UpdateWebsite(Website webSite)
        {
            _siteService.Update(webSite);
            if(webSite.Progress != null)
                _siteService.UpdateProgress(webSite.Progress);
        }
        /// <summary>
        /// Checks if a <see cref="Website"/> exists with the given url.
        /// </summary>
        /// <param name="webSiteUrl">Url of website to check if it exists.</param>
        /// <returns>Whether the <see cref="Website"/> exists.</returns>
        public bool WebsiteExistsWithUrl(string webSiteUrl)
        {
            return _siteService.WebsiteExistsWithUrl(webSiteUrl);
        }

        /// <summary>
        /// Seeds the database with the given urls
        /// </summary>
        /// <param name="whitelist"><see cref="IEnumerable{T}"/> of websites which should never be crawled.</param>
        /// <param name="seed"><see cref="IEnumerable{T}"/> of websites where the program should start.</param>
        public async Task SeedDatabase(IEnumerable<string> whitelist, IEnumerable<string> seed)
        {
            var whitelistedWebsites = whitelist
                .Select(async s => await GetWhitelistedWebsite(s.Replace("\"", "")));
            var websites =await  Task.WhenAll(whitelistedWebsites);
            websites.ToList().ForEach(website =>
            {
                website.ShouldNotBeScraped = true;
                UpdateWebsite(website);
            });
            
            var standardWebsites = seed
                .Select(async s => await GetWebSite(UrlUtils.SanetizeUrl(s)));
            await Task.WhenAll(standardWebsites);
        }

        /// <summary>
        /// Get All websites from database
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> with all websites</returns>
        public IEnumerable<Website> GetAllWebsites()
        {
            return _siteService.GetAll();
        }

        /// <summary>
        /// Use the <see cref="WebParser"/> to get the information out of the page.
        /// </summary>
        /// <param name="content">The page.</param>
        /// <param name="backupUri">Url to fall back on for the host name.</param>
        /// <param name="link">Url of the website.</param>
        /// <returns>A <see cref="WebPage"/>.</returns>
        public WebPage GetWebPage(string content, Uri? backupUri, string link)
        {
            var parser = new WebParser(content);
            WebPageBuilder builder = new();
            return builder.AddLanguage(parser.GetLanguage())
                        .AddPublishingDate(parser.GetDate())
                        .AddTitle(parser.GetTitle())
                        .AddWebsiteName(parser.GetWebsiteName(backupUri))
                        .AddHostName(backupUri?.Host ?? "Not Found")
                        .AddUrl(link)
                        .AddMainContent(parser.GetMainArticle())
                        .GetWebPage();
        }
    }
}
