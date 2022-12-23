using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeNewsBackend.Common;
using FakeNewsBackend.Controller;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
using FakeNewsBackend.Parser;
using FakeNewsBackend.Service.Interface;
using Moq;
using NUnit.Framework;

namespace Tests.Controller;

public class SitemapControllerTest
{
    private SiteMapController _controller;
    private Mock<HttpController> mockedHttp;
    private Mock<ISitemapService> mockedSitemapService;
    private string sitemapContentSitemaps;
    private string sitemapContentLinks;
    private string webpageContent;

    [SetUp]
    public void setup()
    {
        mockedHttp = new Mock<HttpController>(new HttpClient());
        mockedSitemapService = new Mock<ISitemapService>();
        _controller = new(mockedHttp.Object, new Random(1), mockedSitemapService.Object);
        sitemapContentLinks = File.ReadAllText(@"../../../resources/ninefornewsSitemapUrls.xml");
        sitemapContentSitemaps = File.ReadAllText(@"../../../resources/ninefornewsSitemap.xml");
        webpageContent = File.ReadAllText(@"../../../resources/webpageContentTest.txt");
        
        
    }

    [TestCase(true, HttpStatusCode.OK)]
    [TestCase(false, HttpStatusCode.NotFound)]
    [TestCase(false, HttpStatusCode.Forbidden)]
    public async Task WebsiteHasSitemapTest(bool expectedResult, HttpStatusCode code)
    {
        mockedHttp.Setup(i => i.MakeGetRequest(
                It.IsAny<string>(), null).Result)
            .Returns(new HttpDTO()
            { 
                content = sitemapContentSitemaps, 
                Response = new HttpResponseMessage(code)
            });
        var website = new Website() { Url = "ninefornews.com" };

        var result = await _controller.WebsiteHasSitemap(website);
        
        Assert.AreEqual(expectedResult, result);
    }

    [Test]
    public async Task RequestSiteMapTest()
    {
        mockedHttp.Setup(i => i.MakeGetRequest(
                It.IsAny<string>(), null).Result)
            .Returns(new HttpDTO()
            { 
                content = sitemapContentLinks, 
                Response = new HttpResponseMessage(HttpStatusCode.OK)
            });
        var parser = new XmlParser();
        parser.AddDocument(sitemapContentLinks);
        var expected = parser.ParseXml();

        var result = await _controller.RequestSiteMap("ninefornews.com", false);
        
        Assert.NotNull(result);
        Assert.AreEqual(expected.Count(), result.Count());
        foreach (var zip in expected.Zip(result))
        {
            Assert.AreEqual(zip.First.url, zip.Second.url);
            Assert.AreEqual(zip.First.lastmod, zip.Second.lastmod);
        }
    }
    
    [Test]
    public async Task RequestMultipleSiteMapTest()
    {
        var firstCall = true;
        mockedHttp.Setup(i => i.MakeGetRequest(It.IsAny<string>(), null).Result)
            .Returns(() =>
            {
                if (firstCall)
                {
                    firstCall = false;
                    return new HttpDTO()
                    {
                        content = sitemapContentSitemaps,
                        Response = new HttpResponseMessage(HttpStatusCode.OK)
                    };
                }

                return new HttpDTO()
                {
                    content = sitemapContentLinks,
                    Response = new HttpResponseMessage(HttpStatusCode.OK)
                };
                
            });
        var parser = new XmlParser();
        parser.AddDocument(sitemapContentLinks);
        var expected = parser.ParseXml();

        var result = await _controller.RequestSiteMap("ninefornews.com", true);
        
        Assert.NotNull(result);
        // expected count is multiplied by 94, because  there are 94 links which return the same links
        Assert.AreEqual(expected.Count() * 94, result.Count());
        for (int i = 0; i < expected.Count(); i++)
        {
            Assert.AreEqual(expected.ToList()[i].url, result.ToList()[i].url);
            Assert.AreEqual(expected.ToList()[i].lastmod, result.ToList()[i].lastmod);
        }
    }

    [Test]
    public async Task GenerateSitemapTest()
    {
        mockedHttp.Setup(i => i.MakeGetRequest(
                It.IsAny<string>(), null).Result)
            .Returns(new HttpDTO()
            { 
                content = webpageContent, 
                Response = new HttpResponseMessage(HttpStatusCode.OK)
            });
        var website = new Website {Id = 1, Name= "NineForNews.nl", Url = "https://www.ninefornews.nl"};
        var parser = new WebParser(webpageContent);
        
        //This is not filtered
        var allLinksOnPage = parser.GetAllLinksOnPage()
            .Select(dto =>
            {
                if (dto.url.StartsWith("/"))
                    dto.url = website.Url + dto.url;
                return dto;
            });
        
        var result = await _controller.GenerateSitemap(website);
        
        // the number of the useable links on the page
        Assert.AreEqual(122, result.Count());
        foreach (var item in result)
            Assert.True(allLinksOnPage.Contains(item, new DistinctUrlComparer()));
    }
}