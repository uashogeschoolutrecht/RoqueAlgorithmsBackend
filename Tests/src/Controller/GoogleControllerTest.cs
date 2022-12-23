using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeNewsBackend.Controller;
using FakeNewsBackend.Domain.DTO;
using Moq;
using NUnit.Framework;

namespace Tests.Controller;

public class GoogleControllerTest
{
    private GoogleSearchController _controller;
    private Mock<HttpController> mockedHttp;
    private string content;
    
    [SetUp]
    public void setup()
    {
        mockedHttp = new Mock<HttpController>(new HttpClient());
        _controller = new(mockedHttp.Object);

        content = File.ReadAllText(@"../../../resources/googleapi.json");
    }

    [Test]
    public async Task searchTitleTest()
    {
        mockedHttp.Setup(i => i.MakeGetRequest(
                It.IsAny<string>(),null).Result)
            .Returns(new HttpDTO()
            { 
                content = content, 
                Response = new HttpResponseMessage(HttpStatusCode.OK)
            });
        var expected = new List<UrlItemDTO>
        {
            new() { lastmod = DateTime.MinValue, url = "https://test.com/" },
            new() { lastmod = DateTime.MinValue, url = "https://www.americastestkitchen.com/" },
            new() { lastmod = DateTime.MinValue, url = "https://en.wikipedia.org/wiki/Test" },
            new() { lastmod = DateTime.MinValue, url = "https://search.google.com/test/mobile-friendly" },
            new() { lastmod = DateTime.MinValue, url = "https://www.merriam-webster.com/dictionary/test" },
            new() { lastmod = DateTime.MinValue, url = "https://implicit.harvard.edu/implicit/takeatest.html" },
            new() { lastmod = DateTime.MinValue, url = "https://www.cdc.gov/coronavirus/2019-ncov/symptoms-testing/testing.html" },
            new() { lastmod = DateTime.MinValue, url = "https://zoom.us/test" },
            new() { lastmod = DateTime.MinValue, url = "https://www.ets.org/toefl.html" },
            new() { lastmod = DateTime.MinValue, url = "https://www.speedtest.net/" },
        };

        var result = await _controller.SearchTitle("test", "testSite.com");
        
        Assert.AreEqual(result.Count(), expected.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(result.ToList()[i].lastmod, expected[i].lastmod);
            Assert.AreEqual(result.ToList()[i].url, expected[i].url);
        }
    }
}