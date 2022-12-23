using System;
using System.Collections.Generic;
using System.IO;
using FakeNewsBackend.Parser;
using NUnit.Framework;

namespace Tests.Parser;

public class RobotParserTest
{

    private string robotTextFile;

    private RobotParser _parser;

    [SetUp]
    public void setup()
    {
        robotTextFile = File.ReadAllText(@"../../../resources/RobotTestGoogle.txt");
        _parser = new RobotParser();
    }

    [Test]
    public void testDisallowedLinks()
    {
        //arrange
        _parser.AddFile(robotTextFile);
        _parser.ParseRobot();
        var expectedLinks = new List<string>()
        {
            "/?",
            "/books?*zoom=*",
        };
        //act
        var result = _parser.GetDisAllowedRules();

        //assert
        Assert.IsTrue(result.Count == 226);
        for (int i = 0; i < expectedLinks.Count; i++)
        {
            Assert.Contains(expectedLinks[i], result);
        }
    }
    [Test]
    public void testAllowedLinks()
    {
        //arrange
        _parser.AddFile(robotTextFile);
        _parser.ParseRobot();
        var expectedLinks = new List<string>()
        {
            "/?hl=",
            "/books?*q=related:*",
            "/books?*q=editions:*",
            "/books?*q=subject:*",
        };
        //act
        var result = _parser.GetAllowedRules();

        //assert
        Assert.IsTrue(result.Count == 51);
        for (int i = 0; i < expectedLinks.Count; i++)
        {
            Assert.Contains(expectedLinks[i], result);
        }
    }


}