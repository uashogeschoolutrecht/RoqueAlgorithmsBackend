using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Controller;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
using FakeNewsBackend.Parser;
using FakeNewsBackend.Service;
using FakeNewsBackend.Service.Interface;
using Moq;
using NUnit.Framework;

namespace Tests.Controller;

public class WebControllerTest
{
    private Mock<HttpController> mockedHttp;
    private Mock<IWebsiteService> mockedWebsiteService;
    private string webpageContent;
    private WebController _controller;
    private WebPage testPage;
    private HttpResponseMessage response;

    [SetUp]
    public void setup()
    {
        mockedHttp = new Mock<HttpController>(new HttpClient());
        mockedWebsiteService = new Mock<IWebsiteService>();
        _controller = new (mockedHttp.Object, new Random(1), mockedWebsiteService.Object);
        webpageContent = File.ReadAllText(@"../../../resources/webpageContentTest.txt");
        testPage = new()
        {
            Title = "PvdA-Kamerlid Henk Nijboer krijgt snoeiharde kritiek na terugtreden uit presidium: ‘Watjes zijn jullie’",
            DatePosted = DateTime.Parse("2022-10-04T21:30:46+02:00").ToUniversalTime(),
            Language = Language.NL,
            Url = "www.ninefornews.nl",
            HostName = "www.ninefornews.nl",
            WebsiteName= "NineForNews.nl",
            MainContent = new WebParser(File.ReadAllText(@"../../../resources/webpageContentTest.txt"))
                .GetMainArticle()
        };
        response = new HttpResponseMessage(HttpStatusCode.OK);
        response.RequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("https://www.ninefornews.nl"));
    }

    [Test]
    public async Task RequestWebPageTest()
    {
        //arrange
        mockedHttp.Setup(i => i.MakeGetRequest(
                It.IsAny<string>(), null).Result)
            .Returns(new HttpDTO()
            { 
                content = webpageContent, 
                Response = response
            });
        //act
        var results = await _controller.RequestWebPage("www.ninefornews.nl", DateTime.MinValue );

        //assert
        Assert.AreEqual(results, testPage);
    }

    [Test]
    public async Task RequestMultipleArticlesTest()
    {
        mockedHttp.Setup(i => i.MakeGetRequest(
                It.IsAny<string>(), null).Result)
            .Returns(new HttpDTO()
            { 
                content = webpageContent, 
                Response = response
            });
        var siteLists = new List<UrlItemDTO>
        {
            new UrlItemDTO() { lastmod = DateTime.MinValue, url = "www.ninefornews.nl" },
            new UrlItemDTO() { lastmod = DateTime.MinValue, url = "www.ninefornews.nl/tag/" },
            new UrlItemDTO() { lastmod = DateTime.MinValue, url = "www.ninefornews.nl" }
        };

        var results = await _controller.RequestMultipleArticles(siteLists,false);
        
        Assert.AreEqual(results.Count, 2);
        results.ForEach(result => Assert.AreEqual(result, testPage));
    }

    [Test]
    public async Task GetWebsiteTest()
    {
        mockedHttp.Setup(i => i.MakeGetRequest(
                It.IsAny<string>(), null).Result)
            .Returns(new HttpDTO()
            { 
                content = webpageContent, 
                Response = response
            });
        mockedWebsiteService.Setup(i => i.WebsiteExistsWithUrl(
            It.IsAny<string>()))
            .Returns(false);
        mockedWebsiteService.Setup(i => i.GetWebSiteByUrl(It.IsAny<string>()))
            .Returns(new Website
            {
                Id = 1,
                Name = "NineForNews.nl",
                Url = "https://www.ninefornews.nl",
                Rules = new RobotRules(),
                HasSitemap = false,
                HasRules = false
            });
        var website = new Website { 
            Id = 1, 
            Name = "NineForNews.nl", 
            Url = "https://www.ninefornews.nl"
        };

        var result = await _controller.GetWebSite("https://www.ninefornews.nl");
        
        Assert.AreEqual(result, website);
    }

    [Test]
    public void GetWebPageTest()
    {
        var result =
            _controller.GetWebPage(webpageContent, new Uri("https://www.ninefornews.nl"), "www.ninefornews.nl");

        Assert.AreEqual(result, testPage);
    }
}