using System.Text;
using System.Web;
using FakeNewsBackend.Common;
using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Common.Extensions;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Domain.DTO;

namespace FakeNewsBackend.Controller;

public class BingSearchController : ISearchController
{
    private readonly HttpController _httpController;
    private static readonly string API_KEY = Config.GetConnectionString("BingSearchApi");
    private static readonly string BASE_URL = "https://api.bing.microsoft.com/v7.0/search";
    private static readonly int NUMBER_RESULTS = 10;
    private static readonly int BING_SEARCH_LIMIT = 35000;
    private int totalSearches = 0;
    public BingSearchController(HttpController httpController)
    {
        _httpController = httpController;
    }
    public async Task<IEnumerable<UrlItemDTO>> SearchTitle(string title, string originalSite)
    {
        if(totalSearches >= BING_SEARCH_LIMIT)
            System.Environment.Exit(500); 

        var uriBuilder = new UriBuilder(BASE_URL);
        uriBuilder.Query = new StringBuilder()
            .Append($"q=`{HttpUtility.UrlEncode(title)}`")
            .Append($"&mkt=nl-NL")
            .Append($"&answerCount={NUMBER_RESULTS}")
            .ToString();
        var headers = new Dictionary<string, string>();
        headers.Add("Ocp-Apim-Subscription-Key", API_KEY);
        var response = await _httpController.MakeGetRequest(uriBuilder.ToString(), headers);
        totalSearches++;
        
        if(!JsonDocumentExtension.TryParse(response.content, out var jsonResult))
            throw new JsonElementParseException($"Invalid JSON: {response.content}");
        
        if (!jsonResult.RootElement.TryGetProperty("webPages", out var links))
            throw new JsonElementNotFoundException($"Cant find 'webPages' property in JSON: {response.content}");

        return links.GetProperty("value").EnumerateArray()
            .Where(el => el.GetProperty("url").GetRawText().Length > 0)
            .Select(el => new UrlItemDTO()
            {
                url = el.GetProperty("url").GetRawText()
                    .Replace("\"", "")
                    .Replace("\\",""), 
                lastmod = DateTime.MinValue
            })
            .Where(link => !link.url.Contains(originalSite) && 
                           !BlackList.LinkIsInBlackList(link.url, Blacklist.Search));
    }
}