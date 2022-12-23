using System.Text;
using System.Web;
using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Common.Extensions;
using FakeNewsBackend.Domain.DTO;

namespace FakeNewsBackend.Controller;

public class GoogleSearchController : ISearchController
{
    private readonly HttpController _httpController;
    private static readonly string API_KEY = "AIzaSyAYC0759yUvRf4l6o7Y6vBG1JpfwOJGXDM";
    private static readonly string ENGINE_ID = "0557946fcb2b943f3";
    private static readonly string BASE_URL = "https://customsearch.googleapis.com/customsearch/v1";
    private static readonly int NUMBER_RESULTS = 10;
    private const int GOOGLE_SEARCH_LIMIT = 100;
    // public static readonly int GOOGLE_RETRY_DELAY = 3600000;
    public const int GOOGLE_RETRY_DELAY = 3600;
    private int RequestMade;
    private bool Iswaiting;

    public GoogleSearchController(HttpController httpController)
    {
        _httpController = httpController;
        RequestMade = 1;
    }

    public async Task<IEnumerable<UrlItemDTO>> SearchTitle(string title, string originalSite)
    {
        
        if (RequestMade >= GOOGLE_SEARCH_LIMIT && !Iswaiting)
        {
            Iswaiting = true;
            throw new GoogleExhaustedException("The limit of searches has been reached.");
        }

        var uriBuilder = new UriBuilder(BASE_URL);
        uriBuilder.Query = new StringBuilder()
            .Append($"q=`{HttpUtility.UrlEncode(title)}`")
            .Append($"&cx={ENGINE_ID}")
            .Append($"&key={API_KEY}")
            .Append($"&gl=nl")
            .Append($"&num={NUMBER_RESULTS}")
            .Append($"&siteSearch={originalSite}")
            // .Append($"&exactTerms={HttpUtility.UrlEncode(title)}")
            .Append($"&siteSearchFilter=E")
            .ToString();

        var response = await _httpController.MakeGetRequest(uriBuilder.ToString());
        if (response.content == "Unavailable")
            throw new GoogleExhaustedException("API return nothing");
        RequestMade++;
        
        if(!JsonDocumentExtension.TryParse(response.content, out var jsonResult))
            throw new JsonElementParseException($"Invalid JSON: {response.content}");
        
        if (!jsonResult.RootElement.TryGetProperty("items", out var links))
            throw new JsonElementNotFoundException($"Cant find 'items' property in JSON: {response.content}");

        Iswaiting = false;
        RequestMade = 0;
        return links.EnumerateArray()
            .Where(el => el.GetProperty("link").GetRawText().Length > 0)
            .Select(el => new UrlItemDTO()
            {
                url = el.GetProperty("link").GetRawText().Replace("\"", ""), 
                lastmod = DateTime.MinValue
            });
    }
}