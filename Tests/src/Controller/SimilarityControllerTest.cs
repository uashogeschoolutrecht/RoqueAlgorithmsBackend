using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Controller;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
using FakeNewsBackend.Service;
using FakeNewsBackend.Service.Interface;
using Moq;
using NUnit.Framework;

namespace Tests.Controller;

public class SimilarityControllerTest
{
    private SimilarityController _controller;
    private Mock<HttpController> mockedHttp;
    private Mock<ISimilarityService> mockedService;
    private WebPage OriginalPage;
    private WebPage FoundPage;

    [SetUp]
    public void setup()
    {
        mockedHttp = new Mock<HttpController>(new HttpClient());
        mockedService = new Mock<ISimilarityService>();
        _controller = new (mockedHttp.Object,mockedService.Object);
        
        OriginalPage = new ()
        {
            DatePosted = DateTime.MinValue,
            Url = "www.testOriginal.com",
            Language = Language.NL,
            HostName = "testOriginal.com",
            MainContent = "this is the main content",
            Title = "This is the title",
            WebsiteName = "Test Original"
        };
        FoundPage = new ()
        {
            DatePosted = DateTime.MinValue,
            Url = "www.testFound.com",
            Language = Language.NL,
            HostName = "testFound.com",
            MainContent = "this is the main content",
            Title = "This is the title",
            WebsiteName = "Test Found"
        };
    }

    [Test]
    public async Task GetSimilarity()
    {
        //arrange
        mockedHttp.Setup(i => i.MakePostRequest(
                It.IsAny<string>(), 
                It.IsAny<IDictionary<string, string>>()
                ).Result)
            .Returns(new HttpDTO()
            { 
                content = "{\"similarity\": 0.9}", 
                Response = new HttpResponseMessage(HttpStatusCode.OK) 
            });
        //act
        var result = await _controller.GetSimilarityScore(OriginalPage, FoundPage);
        
        //assert
        Assert.True(result.SimilarityScore == 0.9f);
    }
}