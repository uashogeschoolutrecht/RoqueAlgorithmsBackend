using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeNewsBackend.Controller;
using FakeNewsBackend.Domain;
using FakeNewsBackend.Domain.DTO;
using FakeNewsBackend.Service.Interface;
using Moq;
using NUnit.Framework;

namespace Tests.Controller;

public class RobotsControllerTest
{
    private Mock<HttpController> mockedHttp;
    private Mock<IRobotRulesService> mockedRobotRules;
    private string webpageContent;
    private RobotsController _controller;
    private Website testSite;

    [SetUp]
    public void setup()
    {
        mockedHttp = new Mock<HttpController>(new HttpClient());
        mockedRobotRules = new Mock<IRobotRulesService>();
        _controller = new(mockedHttp.Object, mockedRobotRules.Object);
        webpageContent = File.ReadAllText(@"../../../resources/RobotTestGoogle.txt");
        
        testSite =  new Website {Id = 1, Name= "test site", Url = "https://www.test.com"};
    }

    [Test]
    public async Task GetRobotRulesTest()
    {
        mockedHttp.Setup(i => i.MakeGetRequest(
                It.IsAny<string>(), null).Result)
            .Returns(new HttpDTO()
            { 
                content = webpageContent, 
                Response = new HttpResponseMessage()
            });
        mockedRobotRules.Setup(i => i.ExistsWithWebsiteId(
            It.IsAny<int>())).Returns(false);

        var result = await _controller.GetRobotsRules(testSite);
        
        Assert.AreEqual(result.AllowedLinks.Count,51);
        Assert.AreEqual(result.DisallowedLinks.Count,226);
    }

    [Test]
    public async Task GetRobotRulesTestFails()
    {
        mockedHttp.Setup(i => i.MakeGetRequest(
                It.IsAny<string>(), null).Result)
            .Returns(new HttpDTO()
            { 
                content = "404", 
                Response = new HttpResponseMessage(HttpStatusCode.NotFound)
            }); 
        var result = await _controller.GetRobotsRules(testSite);
        
        Assert.Null(result);
    }
}