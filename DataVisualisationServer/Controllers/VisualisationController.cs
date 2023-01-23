using System.Collections.Concurrent;
using System.IO;
using System.Text;

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
        SemaphoreSlim semaphore = new SemaphoreSlim(6);
        var path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                   "/graph.json";
       
        var sims = _similarityController.GetAllSimilarities()
            .Where(s => s.SimilarityScore >= threshold).ToList();
        Console.WriteLine("Filtering websites");
        var websites = _webController.GetAllWebsites()
            .Where(w => sims.Any(s => s.OriginalWebsiteId == w.Id || s.FoundWebsiteId == w.Id))
            .ToList();
        Console.WriteLine("Grouping ");
        var allSims = sims.GroupBy(s => new Tuple<int, int>(s.OriginalWebsiteId, s.FoundWebsiteId))
            .ToDictionary(s => s.Key, s => s.ToList()); 
        var links = new ConcurrentBag<Link>();
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 4 };

        
        var nodes = websites.Select(website => CreateNode(website))
            .ToList();
        Console.WriteLine($"Getting data for {websites.Count} websites");
        var i = 0;
        foreach(var website1 in websites)
        {
            i++;
            Console.WriteLine($"now on websites: {website1.Id} -- loop i :{i}");
            Parallel.ForEach(websites, parallelOptions, website2 =>
            {
                semaphore.Wait();
                try
                {
                    if (website1 == website2)
                        return;

                    var linksBetweenWebsites = allSims[new Tuple<int, int>(website1.Id, website2.Id)].ToArray();

                    if (linksBetweenWebsites.Length == 0)
                        return;
                    GetNode(nodes, website1)?.Increment();
                    GetNode(nodes, website2)?.Increment();
                    links.Add(new Link
                    {
                        source = website1.Id,
                        target = website2.Id,
                        amountOfCopies = linksBetweenWebsites.Length,
                        similarities = linksBetweenWebsites.Select(sim => CreateSim(sim)).ToArray()
                    });
                }
                catch(KeyNotFoundException)
                {
                    return;
                }
                finally
                {
                    semaphore.Release();
                }

            });
        }
        Console.WriteLine("Done with linking");
        nodes = nodes.Where(node => InLinks(links, node))
            .ToList();
        Console.WriteLine($"Making object out of {nodes.Count} nodes");
        var nodeObjs = nodes.Select(website => new 
        {
            website.id,
            website.label,
            website.totalArticles,
            website.amountOfConnections,
            website.totalArticlesScraped
        }).ToArray();
        Console.WriteLine($"Making objects out of {links.Count} links");
        var linkObjs = links.Select(l => new
        {
            l.source,
            l.target,
            l.amountOfCopies,
            l.similarities,
        }).ToArray();
        Console.WriteLine("Making Json file");
        var file = new {
            nodes = nodeObjs,
            links = linkObjs
        };
        var opt = new JsonSerializerOptions();
        opt.WriteIndented = true;
        

        string filecontent = JsonSerializer.Serialize(file,opt);
        System.IO.File.WriteAllText(path, filecontent);
        return filecontent;

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
            totalArticlesScraped = website.NumberOfArticlesScraped,
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