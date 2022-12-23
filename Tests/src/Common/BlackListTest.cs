using FakeNewsBackend.Common;
using NUnit.Framework;

namespace Tests.Common;

public class BlackListTest
{
    [TestCase("www.test.nl/post", false)]
    [TestCase("www.test.nl/tag", true)]
    [TestCase("www.test.nl/agenda", true)]
    [TestCase("www.test.nl/post.pdf", true)]
    [TestCase("www.test.nl/post.xml", false)]
    [TestCase("https://www.hetnieuwsmaardananders.nl/nl/binnenland/667066/", false)]
    public void TestLinkIsInBlackList(string link, bool expectedResult)
    {
        Assert.AreEqual(expectedResult, BlackList.LinkIsInBlackList(link));
    }
}