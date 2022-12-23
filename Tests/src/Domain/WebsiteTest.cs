using System.Collections.Generic;
using FakeNewsBackend.Domain;
using NUnit.Framework;

namespace Tests.Domain;

public class WebsiteTest
{
    private RobotRules _rules;
    private IDictionary<int, List<string>> differentDisAllowedLinks = new Dictionary<int, List<string>>()
    {
        { 0, new List<string>() {"disallowed" } },
        { 1, new List<string>() { "/comments/" } },
    };
    private IDictionary<int, List<string>> differentAllowedLinks = new Dictionary<int, List<string>>()
    {
        { 0, new List<string>() { "allowed" } },
        { 1, new List<string>() { "/comments/test" } },
    };
    
    
    [TestCase(0,0,true, "test.com/allowed")]
    [TestCase(0,0,false, "test.com/disallowed")]
    [TestCase(1,1,false, "test.com/comments/notAllowed")]
    [TestCase(1,1,true, "test.com/comments/test")]
    public void TestIsAllowed(int numberDisAllowedLinks, int numberAllowedLinks, bool isAllowed, string linkToTest)
    {
        //arrange
        _rules = new RobotRules();
        var site = new Website();
        
        //act
        differentDisAllowedLinks[numberDisAllowedLinks].ForEach(link => _rules.AddDisAllowedLink(link));
        differentAllowedLinks[numberAllowedLinks].ForEach(link => _rules.AddAllowedLink(link));
        site.AddRules(_rules);
        //assert
        if(isAllowed)
            Assert.True(site.IsLinkAllowed(linkToTest)); 
        else 
            Assert.False(site.IsLinkAllowed(linkToTest));
    }
}