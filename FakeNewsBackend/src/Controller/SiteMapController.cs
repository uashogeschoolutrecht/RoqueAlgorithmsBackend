using System.Net;
using System.Xml;
using FakeNewsBackend.Common;
using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
using FakeNewsBackend.Parser;
using FakeNewsBackend.Service.Interface;
using NLog;

namespace FakeNewsBackend.Controller;

public class SiteMapController
{
    private readonly HttpController _httpController;
    private readonly ISitemapService _sitemapService;
    private readonly Random _random;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private DateTime timeOffset = new(2020, 1, 1);
    
    public SiteMapController(HttpController httpController, Random random, ISitemapService sitemapService)
    {
        _httpController = httpController;
        _random = random;
        _sitemapService = sitemapService;
    }
    /// <summary>
    /// Request sitemap file from website.
    /// </summary>
    /// <param name="site">Url of the sitemap.</param>
    /// <param name="initialSitemap">If the call is for the first time.</param>
    /// <returns>A <see cref="Task"/> with an <see cref="IEnumerable{T}"/>
    ///     containing all the links of the sitemaps as <see cref="UrlItemDTO"/>.</returns>
    public async Task<IEnumerable<UrlItemDTO>?> RequestSiteMap(string site, bool initialSitemap)
    {
        try
        {
            string urlToRequest;
            if (initialSitemap)
                urlToRequest = site + "/sitemap.xml";
            else
                urlToRequest = site;
            var requestResult = await _httpController.MakeGetRequest(urlToRequest);

            if (!requestResult.Response.IsSuccessStatusCode)
                Console.WriteLine("hi");

            var parser = new XmlParser();
            parser.AddDocument(requestResult.content);

            var xml = parser.ParseXml();

            switch (parser.GetXmlType())
            {
                case XmlType.SITEMAP_SITEMAP:
                    return await RequestMultipleSitemaps(xml.ToList());
                case XmlType.SITEMAP_ARTICLES:
                    return xml.ToList();
                case XmlType.SITEMAP_CATEGORIES: break;
                case XmlType.SITEMAP_AUTHOR: break;
                case XmlType.SITEMAP_TAG: break;
                default:
                    _logger.Warn("Xml parsing failed: {Site}", site);
                    break;
            }
        }
        catch (XmlException e)
        {
            _logger.Error(e,"Xml parsing failed: {Site}", site);
        }
        catch (SiteMapException e)
        {
            _logger.Error(e,"There was no sitemap document for: {Site}", site);
            Console.WriteLine(e);
        }
        return new List<UrlItemDTO>();
    }

    /// <summary>
    /// Get the sitemap form <see cref="Website"/>.
    /// </summary>
    /// <param name="webSite">The website to get the sitemap from.</param>
    /// <returns>A <see cref="Task"/> with the <see cref="Sitemap"/>.</returns>
    public async Task<Sitemap> GetSitemapOfWebsite(Website webSite)
    {
        if (webSite.HasSitemap)
            return _sitemapService.GetSitemapByWebsiteId(webSite.Id);
        
        var hasSitemap = await WebsiteHasSitemap(webSite);
        
        var sitemapLinks =  hasSitemap ? 
            await RequestSiteMap(webSite.Url, true) : 
            await GenerateSitemap(webSite);

        if (hasSitemap && sitemapLinks.ToList().Count == 0)
        {
            _logger.Debug("Sitemap of {Website} was empty",webSite.Name);
            sitemapLinks = await GenerateSitemap(webSite);
        } 

        var items = sitemapLinks.Select(dto => new SitemapItem()
            { date = dto.lastmod, url = dto.url, websiteId = webSite.Id });
        var siteMap = new Sitemap() { links = items.Distinct() , WebsiteId = webSite.Id };
        
        _sitemapService.Add(siteMap);
        
        _logger.Trace("Got sitemap for: {Website}", webSite.Name);
        
        return siteMap;
    }

    /// <summary>
    /// Check if the <see cref="Website"/> has a sitemap file.
    /// </summary>
    /// <param name="website">The <see cref="Website"/> to check.</param>
    /// <returns>A <see cref="Task"/> containing whether the <see cref="Website"/> has a sitemap file.</returns>
    public async Task<bool> WebsiteHasSitemap(Website website)
    {
        var link = website.Url;
        if (link.EndsWith('/'))
            link += "sitemap.xml";
        else
            link += "/sitemap.xml";
        var requestResult = await _httpController.MakeGetRequest(link);
        return requestResult.Response.StatusCode == HttpStatusCode.OK;
    }
    /// <summary>
    /// Request multiple sitemaps.
    /// (Some websites have multiple sitemaps)
    /// </summary>
    /// <param name="items">A <see cref="List{T}"/> containing urls of the sitemaps as <see cref="UrlItemDTO"/></param>
    /// <returns>A <see cref="Task"/> with an <see cref="IEnumerable{T}"/>
    ///     containing all the links of the sitemaps as <see cref="UrlItemDTO"/>.</returns>
    /// <exception cref="SiteMapException"></exception>
    public async Task<IEnumerable<UrlItemDTO>> RequestMultipleSitemaps(List<UrlItemDTO> items)
    {
        List<Task<IEnumerable<UrlItemDTO>>> sitemaps = new();
        for (var i = 0; i < items.Count; i++)
        {
            if(BlackList.LinkIsInBlackList(items[i].url))
                continue;
            if(i % 10 == 0)
                Thread.Sleep(_random.Next(5000, 8000));
            Console.WriteLine("Making sitemap Request: " + items[i].url);
            Thread.Sleep(_random.Next(500, 1000));
            sitemaps.Add(RequestSiteMap(items[i].url,false));
        }
        var maps = await Task.WhenAll(sitemaps);
        Console.WriteLine("sitemaps requested: " + maps.Length);
        if (maps.Length == 0 | maps == null)
            throw new SiteMapException("No sitemaps");

        return maps.ToList()
            .Aggregate((item, list) => list.Concat(item))
            .Where(i => IsRecentEnough(i.lastmod, i.url));
    }

    /// <summary>
    /// Creates a sitemap when a website does not have a sitemap file.
    /// </summary>
    /// <param name="website">The <see cref="Website"/> to create the sitemap for.</param>
    /// <returns>A <see cref="Task"/> with an <see cref="IEnumerable{T}"/>
    ///     containing all the links of the sitemaps as <see cref="UrlItemDTO"/>.</returns>
    public async Task<IEnumerable<UrlItemDTO>> GenerateSitemap(Website website)
    {
        _logger.Info("Generating sitemap for: {Name}",website.Name);
        var comparer = new DistinctUrlComparer();
        SitemapGenerateProgress prog;
        if (_sitemapService.ProgressExists(website.Id))
            prog = _sitemapService.GetProgress(website.Id);
        else
        {
            prog = new ()
            {
                WebsiteId = website.Id,
                LinksVisited = new List<UrlItemDTO>(),
                LinksWithArticles = new List<UrlItemDTO>()
            };
            var requestResult = await _httpController.MakeGetRequest(website.Url);
            var parser = new WebParser(requestResult.content);
            prog.LinksOnWebsite = parser.GetAllLinksOnPage()
                .Where(linkOnPage => 
                    IsLinkInSameHost(website.Url, linkOnPage.url) && 
                    !BlackList.LinkIsInBlackList(linkOnPage.url, Blacklist.Generating))
                .Distinct(comparer)
                .ToList();
            parser.Dispose();
        }
        Console.WriteLine($"{prog.LinksOnWebsite.Count} links to check");
        
        for (int i = 0, requestMade = 0; i < prog.LinksOnWebsite.Count; i++)
        {
            if (prog.LinksOnWebsite[i].url.StartsWith("/"))
                prog.LinksOnWebsite[i].url = website.Url + prog.LinksOnWebsite[i].url;
            
            if (prog.LinksVisited.Any(pr => pr.url == prog.LinksOnWebsite[i].url) ||
                !website.IsLinkAllowed(prog.LinksOnWebsite[i].url) || 
                !IsLinkInSameHost(website.Url, prog.LinksOnWebsite[i].url) )
                continue;
            
            if (requestMade % 20 == 0 && requestMade != 0)
            {
                Console.WriteLine("Saving a batch of links");
                Console.WriteLine($"{i} out of {prog.LinksOnWebsite.Count} links checked");
                _sitemapService.SaveGenerateProgress(prog);
                Thread.Sleep(_random.Next(10000,15000));
            }
            Console.WriteLine($"Now on: {prog.LinksOnWebsite[i].url}");
            prog.LinksVisited.Add(prog.LinksOnWebsite[i]);
            var requestResultLoop = await _httpController.MakeGetRequest(prog.LinksOnWebsite[i].url);
            requestMade++;
            if (requestResultLoop.Response.StatusCode == HttpStatusCode.Locked)
                Thread.Sleep(_random.Next(30000));
            if (!requestResultLoop.Response.IsSuccessStatusCode || requestResultLoop.content == "Unavailable")
                continue;
            
            using (var tempParser = new WebParser(requestResultLoop.content))
            {
                prog.LinksOnWebsite = prog.LinksOnWebsite.Union(tempParser.GetAllLinksOnPage())
                    .Where(linkOnPage => 
                        IsLinkInSameHost(website.Url, linkOnPage.url) && 
                        !BlackList.LinkIsInBlackList(linkOnPage.url, Blacklist.Generating))
                    .Select(dto =>
                    {
                        if (dto.url.StartsWith("/"))
                            dto.url = website.Url + dto.url;
                        return dto;
                    })
                    .Distinct(comparer)
                    .ToList();
                if (!tempParser.HasOnlyOneArticle())
                    continue;
                prog.LinksOnWebsite[i].lastmod = tempParser.GetDate(); 
                prog.LinksWithArticles.Add(prog.LinksOnWebsite[i]);
            }
        }
        _sitemapService.DeleteGenerateProgress(prog);
        
        _logger.Trace("Generated sitemap for: {Website}", website.Name);
        
        return prog.LinksWithArticles
            .Where(i => IsRecentEnough(i.lastmod, i.url));
    }
    /// <summary>
    /// Update <see cref="SitemapItem"/> in database.
    /// </summary>
    /// <param name="item">The item to update.</param>
    public void UpdateSitemapItem(SitemapItem item)
    {
        _sitemapService.UpdateItem(item);
    }

    /// <summary>
    /// Checks if the given url is of the same host.
    /// </summary>
    /// <param name="host">Url of the host</param>
    /// <param name="link">Url to check</param>
    /// <returns>Whether the <paramref name="link"/> is from the <paramref name="host"/>.</returns>
    private bool IsLinkInSameHost(string host, string link)
    {
        return link.StartsWith(host) || (link.StartsWith("/") && !link.Contains('.'));
    }

    public bool IsRecentEnough(DateTime dateToCheck, string url)
    {
        if (!UrlUtils.UrlHasDate(url))
            return DateTime.Compare(dateToCheck, timeOffset) >= 0 ||
                   DateTime.Compare(dateToCheck, DateTime.MinValue) == 0;
        var year = UrlUtils.GetYearOutOfUrl(url);
        return timeOffset.Year <= year;
    }
}