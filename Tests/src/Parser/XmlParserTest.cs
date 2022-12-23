using System;
using System.Collections.Generic;
using System.Linq;
using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Domain.DTO;
using FakeNewsBackend.Parser;
using NUnit.Framework;

namespace Tests.Parser
{
    internal class XmlParserTest
    {
        [Test]
        public void ParserHasNoXml()
        {
            Assert.Throws<XmlDocumentException>(() => 
                new XmlParser().AddDocument(""));
            Assert.Throws<XmlDocumentException>(() => 
                _ = new XmlParser()
                    .GetXmlType());
        }
        
        [TestCase("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\" xmlns:mobile=\"http://www.google.com/schemas/sitemap-mobile/1.0\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\" xmlns:video=\"http://www.google.com/schemas/sitemap-video/1.1\">\\r\\n<url><loc>https://www.foxnews.com/us/vets-in-training-helping-elderly-keep-animals</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url>\r\n<url>\r\n<loc>https://www.foxnews.com/us/summary-box-layoffs-slow-consumers-spend-more</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url>\r\n<url>\r\n<loc>https://www.foxnews.com/us/epa-moving-unilaterally-to-limit-greenhouse-gases</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url>\r\n<url>\r\n<loc>https://www.foxnews.com/us/parents-2-children-found-dead-in-mississippi-home</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url>\r\n<url>\r\n<loc>https://www.foxnews.com/us/oklahoma-soldiers-clemency-request-denied</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url></urlset>"
        , false)]
        [TestCase("<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\" xmlns:mobile=\"http://www.google.com/schemas/sitemap-mobile/1.0\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\" xmlns:video=\"http://www.google.com/schemas/sitemap-video/1.1\">\\r\\n<sitemap><loc>https://www.foxnews.com/us/vets-in-training-helping-elderly-keep-animals</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</sitemap>\r\n<sitemap>\r\n<loc>https://www.foxnews.com/us/summary-box-layoffs-slow-consumers-spend-more</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</sitemap>\r\n<sitemap>\r\n<loc>https://www.foxnews.com/us/epa-moving-unilaterally-to-limit-greenhouse-gases</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</sitemap>\r\n<sitemap>\r\n<loc>https://www.foxnews.com/us/parents-2-children-found-dead-in-mississippi-home</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</sitemap>\r\n<sitemap>\r\n<loc>https://www.foxnews.com/us/oklahoma-soldiers-clemency-request-denied</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</sitemap></sitemapindex>"
        , true)]
        public void TestXmlParse(string xml, bool isSiteMap)
        {
            //arrange
            IEnumerable<UrlItemDTO> expectedXml = new List<UrlItemDTO>
            {
                new UrlItemDTO { url = "https://www.foxnews.com/us/vets-in-training-helping-elderly-keep-animals", lastmod = DateTime.Parse("2014-11-17T13:16:57-05:00").ToUniversalTime()}, 
                new UrlItemDTO { url = "https://www.foxnews.com/us/summary-box-layoffs-slow-consumers-spend-more", lastmod = DateTime.Parse("2014-11-17T13:16:57-05:00").ToUniversalTime()},
                new UrlItemDTO { url = "https://www.foxnews.com/us/epa-moving-unilaterally-to-limit-greenhouse-gases",lastmod = DateTime.Parse("2014-11-17T13:16:57-05:00").ToUniversalTime()},
                new UrlItemDTO { url = "https://www.foxnews.com/us/parents-2-children-found-dead-in-mississippi-home", lastmod = DateTime.Parse("2014-11-17T13:16:57-05:00").ToUniversalTime()},
                new UrlItemDTO { url = "https://www.foxnews.com/us/oklahoma-soldiers-clemency-request-denied", lastmod = DateTime.Parse("2014-11-17T13:16:57-05:00").ToUniversalTime()},
            };
            XmlParser parser = new XmlParser();
            parser.AddDocument(xml);

            //act
            var actualResult = parser.ParseXml();

            //assert
            Assert.AreEqual(expectedXml.Count(), actualResult.Count());
            foreach (var results in expectedXml.Zip(actualResult,Tuple.Create))
            {
                Assert.AreEqual(results.Item1.url,results.Item2.url);
                Assert.AreEqual(results.Item1.lastmod,results.Item2.lastmod);
            }
        }

        [Test]
        public void TestXmlParseOfSiteMap()
        {
            
        }

        [Test]
         public void TestXmlParseWithoutLink()
        {
            //arrange
            const string xml = "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\" xmlns:mobile=\"http://www.google.com/schemas/sitemap-mobile/1.0\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\" xmlns:video=\"http://www.google.com/schemas/sitemap-video/1.1\">\\r\\n<url><loc>https://www.foxnews.com/us/vets-in-training-helping-elderly-keep-animals</loc>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url>\r\n<url>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url>\r\n<url>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url>\r\n<url>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url>\r\n<url>\r\n<lastmod>2014-11-17T13:16:57-05:00</lastmod>\r\n<changefreq>monthly</changefreq>\r\n<priority>0.5</priority>\r\n</url></urlset>";
            IEnumerable<UrlItemDTO> expectedXml = new List<UrlItemDTO>
            {
                new UrlItemDTO { url = "https://www.foxnews.com/us/vets-in-training-helping-elderly-keep-animals", lastmod = DateTime.Parse("2014-11-17T13:16:57-05:00").ToUniversalTime() }
            };
            var parser = new XmlParser();
            parser.AddDocument(xml);

            //act
            var actualResult = parser.ParseXml();

            //assert
            Assert.AreEqual(expectedXml.Count(), actualResult.Count());
            foreach (var results in expectedXml.Zip(actualResult,Tuple.Create))
            {
                Assert.AreEqual(results.Item1.url,results.Item2.url);
                Assert.AreEqual(results.Item1.lastmod,results.Item2.lastmod);
            }
        }

         [TestCase("<urlset><url><loc>www.test.com/animals</loc></url></urlset>", XmlType.SITEMAP_ARTICLES)]
         [TestCase("<urlset><url><loc>www.test.com/animals.xml</loc></url></urlset>", XmlType.SITEMAP_SITEMAP)]
         [TestCase("<urlset><url><loc>www.test.com/animals</loc></url></urlset>", XmlType.SITEMAP_ARTICLES)]
         [TestCase("<urlset><url><loc>www.test.com/author/animals</loc></url></urlset>", XmlType.SITEMAP_AUTHOR)]
         [TestCase("<urlset><url><loc>www.test.com/tag/animals</loc></url></urlset>", XmlType.SITEMAP_TAG)]
         [TestCase("<urlset><url><loc>www.test.com/category/animals</loc></url></urlset>", XmlType.SITEMAP_CATEGORIES)]
         public void TestXmlType(string xml, XmlType expectedResult)
         {
             XmlParser parser = new XmlParser();
             parser.AddDocument(xml);

             var actualResult = parser.GetXmlType();

             Assert.AreEqual(expectedResult, actualResult);
         }
    }
}
