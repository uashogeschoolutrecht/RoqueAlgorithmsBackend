using System;
using System.Globalization;
using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Parser;
using HtmlAgilityPack;
using NUnit.Framework;

namespace Tests.Parser
{
    internal class WebParserTest
    {

        [Test]
        public void ParserGetsNoHtmlPage()
        {
            Assert.Throws<HtmlDocumentException>(() => 
                _ = new WebParser(""));
        }

        [TestCase("en",Language.EN)]
        [TestCase("en-us",Language.EN)]
        [TestCase("nl",Language.NL)]
        [TestCase("",Language.UNKNOWN)]
        public void PageHasLanguage(string languageInHtml, Language ExpectedLanguage)
        {
            //arrange
            var html = "<html lang=\""+ languageInHtml +"\"><html>";
            WebParser parser;

            //act
            parser = new WebParser(html);
            var foundLanguage = parser.GetLanguage();

            //assert
            Assert.AreEqual(foundLanguage,ExpectedLanguage);
        }
        [Test]
        public void PageDoesNotHaveLanguage()
        {
            //arrange
            var html = "<html><html>";
            WebParser parser;

            //act
            parser = new WebParser(html);
            var foundLanguage = parser.GetLanguage();

            //assert
            Assert.AreEqual(foundLanguage, Language.UNKNOWN);
        }

        [Test]
        public void PageHasTitle()
        {
            //arrange
            var testTitle = "test";
            var html = $"<html><h1>{testTitle}</h1><html>";
            WebParser parser;

            //act
            parser = new WebParser(html);
            var foundTitle = parser.GetTitle();

            //assert
            Assert.AreEqual(foundTitle, testTitle);
        }
        [Test]
        public void PageDoesNotHaveTitle()
        {
            //arrange
            var html = "<html><html>";
            WebParser parser;

            //act
            parser = new WebParser(html);
            var foundTitle = parser.GetTitle();

            //assert
            Assert.AreEqual(foundTitle, "Not Found");
        }

        [TestCase("<html><div>This is outside the article<article>this is inside the article</article></div></html>","this is inside the article")]
        [TestCase("<html><div>This is outside the article<div class=\"article\">this is inside the article</div></div></html>","this is inside the article")]
        [TestCase("<html><div>This is outside the article<div class=\"content\">this is inside the article</div></div></html>","this is inside the article")]
        [TestCase("<html><div>There is no article</div></html>","Not Found")]
        public void PageHasArticle(string html, string expectedResult)
        {
            //arrange
            WebParser parser;
            //act
            parser = new WebParser(html);
            var foundArticle = parser.GetMainArticle();

            //assert
            Assert.AreEqual(expectedResult, foundArticle);
        }

        [TestCase("<html><div>This is outside the time<time>08-09-2022</time></div></html>","08-09-2022")]
        [TestCase("<html><div>This is outside the article<div class=\"date\">08-09-2022</div></div></html>","08-09-2022")]
        [TestCase("<html><div>This is outside the article<div class=\"publish\">08-09-2022</div></div></html>", "08-09-2022")]
        [TestCase("<html><div>This is outside the article<div class=\"publish\">3 de octubre de 2017</div></div></html>", "")]
        [TestCase("<html><div>There Is no time</div></html>", "")]
        public void TestDate(string html, string expectedDateString)
        {
            //arrange
            WebParser parser;
            DateTime expectedDate = DateTime.MinValue;
            
            //act
            if (expectedDateString != "")
            {
                var languages = new[] { "nl-NL", "en-US" };
                foreach (var lang in languages)
                {
                    if (!DateTime.TryParse(expectedDateString, new CultureInfo(lang, false),DateTimeStyles.None, out expectedDate))
                        continue;
                    break;
                }
            }
                
            parser = new WebParser(html);
            DateTime? result = parser.GetDate();

            var toCheck = expectedDate != DateTime.MinValue
                ? expectedDate.ToUniversalTime().ToString("d/M/yyyy", new CultureInfo("nl-NL", false))
                : DateTime.MinValue.ToUniversalTime().ToString("d/M/yyyy", new CultureInfo("nl-NL", false));
            //assert
            Assert.AreEqual(toCheck, 
                result?.ToString("d/M/yyyy", new CultureInfo("nl-NL", false)));
        }

        [TestCase("<html><div>This is <script>scribble</script>a test</div></html>", "<html><div>This is a test</div></html>")]
        [TestCase("<html><div>This is <img><div class=\"ads\">this is an ad</div>a test</div></html>", "<html><div>This is a test</div></html>")]
        [TestCase("<html><div>This is <script>scribble</script>a test</div></html>", "<html><div>This is a test</div></html>")]
        [TestCase("", null)]
        public void TestFilter(string htmlToFilter, string expectedResult)
        {
            //arrange
            var testHtml = new HtmlDocument();
            testHtml.LoadHtml(htmlToFilter);

            //act
            var filteredHtml = WebParser.FilterArticle(testHtml.DocumentNode);

            //assert
            Assert.AreEqual( expectedResult, filteredHtml?.OuterHtml);
        }

        [TestCase("<html><div>This is outside the article<article>This is inside the article</article></div></html>", "<article>This is inside the article</article>")]
        [TestCase("<html><div>This is outside the article<div class=\"article\">This is inside the article</div></div></html>", "<div class=\"article\">This is inside the article</div>")]
        [TestCase("<html><div>This is outside the article<div class=\"content\">This is inside the article</div></div></html>", "<div class=\"content\">This is inside the article</div>")]
        [TestCase("<html><div>This is outside the article<div id=\"content\">This is inside the article</div></div></html>", "<div id=\"content\">This is inside the article</div>")]
        [TestCase("<html><div>This is outside the article<div id=\"article\">This is inside the article</div></div></html>", "<div id=\"article\">This is inside the article</div>")]
        [TestCase("<html><div>There Is no article</div></html>", null)]
        public void TestFindArticle(string htmlToSearch, string expectedResult)
        {
            //arrange
            var parser = new WebParser(htmlToSearch);

            //act
            var foundHtmlNode = parser.FindArticleTag();

            //assert
            Assert.AreEqual(expectedResult, foundHtmlNode?.OuterHtml);
        }
        [TestCase("<html><div>This is outside the time<time>08-09-2022</time></div></html>", "<time>08-09-2022</time>")]
        [TestCase("<html><div>This is outside the article<div class=\"date\">08-09-2022</div></div></html>", "<div class=\"date\">08-09-2022</div>")]
        [TestCase("<html><div>This is outside the article<div class=\"publish\">08-09-2022</div></div></html>", "<div class=\"publish\">08-09-2022</div>")]
        [TestCase("<html><div>There Is no time</div></html>", null)]
        public void TestFindDate(string htmlToSearch, string expetedResult)
        {
            //arrange
            var parser = new WebParser(htmlToSearch);

            //act
            var foundDate = parser.FindDate();

            //assert
            Assert.AreEqual(expetedResult, foundDate?.OuterHtml);
        }

        [TestCase("<html><head><meta property=\"og:site_name\" content=\"test.nl\"><meta property=\"og:locale\" content=\"nl_NL\"></head><div>This is a test</div></html>", "test.nl", null)]
        [TestCase("<html><div>This is a test</div></html>", "www.test.nl", "https://www.test.nl")]
        public void TestWebsiteName(string html,string expectedResult, string? backupUri)
        {
            //arrange
            var parser = new WebParser(html);
            //act
            var foundSiteName = backupUri != null ?
                parser.GetWebsiteName(new Uri(backupUri)) :
                parser.GetWebsiteName(null);
            //assert
            Assert.AreEqual(expectedResult, foundSiteName);
        }

        [TestCase("<html><h1>This is a test</h1></html>", "<h1>This is a test</h1>")]
        [TestCase("<html><h2>This is a test</h2></html>", "<h2>This is a test</h2>")]
        [TestCase("<html><div>This is outside the article<div class=\"title\">This is a test</div></div></html>","<div class=\"title\">This is a test</div>")]
        [TestCase("<html><div>There Is no title</div></html>", null)]
        public void TestFindTitle(string html, string expectedResult)
        {
            //arrange
            var parser = new WebParser(html);

            //act
            var foundHtmlNode = parser.FindTitle();

            //assert
            Assert.AreEqual(expectedResult, foundHtmlNode?.OuterHtml);
        }
    }

}
