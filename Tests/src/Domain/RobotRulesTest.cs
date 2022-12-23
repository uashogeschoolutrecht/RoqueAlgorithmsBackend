
using System.Collections.Generic;
using FakeNewsBackend.Domain;
using NUnit.Framework;

namespace Tests.Domain;

public class RobotRulesTest
{
    private RobotRules rules;

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
        rules = new RobotRules();
        
        //act
        differentDisAllowedLinks[numberDisAllowedLinks].ForEach(link => rules.AddDisAllowedLink(link));
        differentAllowedLinks[numberAllowedLinks].ForEach(link => rules.AddAllowedLink(link));
        //assert
        if(isAllowed)
            Assert.True(rules.IsLinkAllowed(linkToTest)); 
        else 
            Assert.False(rules.IsLinkAllowed(linkToTest));
    }
    
    
}