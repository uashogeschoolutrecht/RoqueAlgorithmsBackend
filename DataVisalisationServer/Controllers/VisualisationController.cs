using System.Collections.Concurrent;
using System.Text.Json;
using FakeNewsBackend.Common;
using Microsoft.AspNetCore.Mvc;
using FakeNewsBackend.Controller;
using FakeNewsBackend.DataVisualisation;
using FakeNewsBackend.Domain;

namespace DataVisalisationServer.Controllers;

[ApiController]
[Route("/")]
public class VisualisationController : ControllerBase
{
    private struct Link
    {
        public int source;
        public int target;
        public int amountOfCopies;
        public object[] similarities;
    }
    
    private SimilarityController _similarityController;
    private WebController _webController;

    public VisualisationController(WebController webController, SimilarityController similarityController)
    {
        _similarityController = similarityController;
        _webController = webController;
    }

    [HttpGet(Name = "GetGraph")]
    public string Get()
    {
        return GetGraphData();
    }
    
    [HttpGet("{Threshold}")]
    public string GetWithThreshold(Single Threshold)
    {
        return GetGraphData(Threshold);
    }
    
    [NonAction]  
    public string GetGraphData(Single threshold = 0.0f)
    {
        var path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                   "/graph.json";
        var websites = _webController.GetAllWebsites().ToList();
        var nodes = websites.Select(website => CreateNode(website))
            .ToList();
        var links = new ConcurrentBag<Link>();

        foreach (var website1 in websites)
            Parallel.ForEach(websites, website2 =>
            {
                if (website1 == website2)
                    return;
                var sims = _similarityController.GetSimilaritiesFromTwoWebsites(website1, website2)
                    .Where(sim => sim.SimilarityScore >= threshold)
                    .ToArray();

                if (sims.Length == 0)
                    return;
                GetNode(nodes, website1)?.Increment();
                GetNode(nodes, website2)?.Increment();
                links.Add(new Link
                {
                    source = website1.Id,
                    target = website2.Id,
                    amountOfCopies = sims.Length,
                    similarities = sims.Select(sim => CreateSim(sim)).ToArray()
                });
            });

        nodes = nodes.Where(node => InLinks(links, node))
            .ToList();

        var file = new {
            nodes = nodes.Select(website => new{
                website.id,
                website.label,
                website.totalArticles,
                website.amountOfConnections
            }).ToArray(),
            links = links.Select(l => new
            {
                l.source,
                l.target,
                l.amountOfCopies,
                l.similarities,
            }).ToArray()
        };
        var opt = new JsonSerializerOptions();
        opt.WriteIndented = true;
        return JsonSerializer.Serialize(file,opt);
    }
    
    [NonAction]  
    public async Task SetSimilaritiesInJson()
    {
        var sims = _similarityController.GetAllSimilarities();
        var path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                   "/similarities.json";
        var list = new List<object>();
        foreach (var sim in sims)
        {
            var orPage = await _webController.RequestWebPage(sim.UrlToOriginalArticle);
            var foPage = await _webController.RequestWebPage(sim.UrlToFoundArticle);
            list.Add(new
            {
                originalArticle = orPage.MainContent,
                foundArticle = foPage.MainContent,
                simirarityScore = sim.SimilarityScore,
                originalUrl = sim.UrlToOriginalArticle,
                foundUrl = sim.UrlToFoundArticle,
            });
        }
        var opt = new JsonSerializerOptions();
        opt.WriteIndented = true;
        var json = JsonSerializer.Serialize(list,opt);
    }

    [NonAction]  
    private Node? GetNode(IEnumerable<Node> nodes, Website website)
    {
        foreach (var node in nodes)
            if (node.id == website.Id)
                return node;
        return null;
    }

    [NonAction]  
    private bool InLinks(IEnumerable<Link> links, Node node)
    {
        foreach (var link in links)
        {
            if (link.source == node.id ||
                link.target == node.id)
                return true;
        }

        return false;
    }
    
    [NonAction]  
    private Node CreateNode(Website website)
    {
        return new Node()
        {
            id = website.Id,
            label = UrlUtils.RemoveProtocol(website.Url),
            totalArticles = website.NumberOfArticles,
            amountOfConnections = 0
        };
    }

    [NonAction]  
    private object CreateSim(Similarity sim)
    {
        return new
        {
            originalUrl = sim.UrlToOriginalArticle,
            foundUrl = sim.UrlToFoundArticle,
            score = sim.SimilarityScore,
            originalDate = sim.OriginalPostDate,
            foundDate = sim.FoundPostDate,
            originalLan = Mapper.LanguageToString(sim.OriginalLanguage),
            foundLan = Mapper.LanguageToString(sim.FoundLanguage)
        };
    }
}