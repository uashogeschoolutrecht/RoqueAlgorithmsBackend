using System;
using FakeNewsBackend.Builder;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Domain;
using NUnit.Framework;

namespace Tests.Builder
{
    internal class WebPageBuilderTest
    {
        [Test]
        public void AddPublishingDate()
        {
            //arrange
            var testDate = DateTime.Parse("7 september 2022");
            var pageBuilder = new WebPageBuilder();

            //act

            //assert
            Assert.IsTrue(pageBuilder.AddPublishingDate(testDate) is WebPageBuilder);
        }
        
        [Test]
        public void AddUrl()
        {
            //arrange
            var testUrl = "test.com";
            var pageBuilder = new WebPageBuilder();

            //act
            pageBuilder = pageBuilder.AddUrl(testUrl);

            //assert
            Assert.IsTrue(pageBuilder.AddUrl(testUrl) is WebPageBuilder);
            Assert.AreEqual(testUrl, pageBuilder.GetWebPage().Url);
        }
        
        [Test]
        public void AddHostName()
        {
            //arrange
            var testHostName = "test.com";
            var pageBuilder = new WebPageBuilder();

            //act
            pageBuilder = pageBuilder.AddHostName(testHostName);

            //assert
            Assert.IsTrue(pageBuilder.AddHostName(testHostName) is WebPageBuilder);
            Assert.AreEqual(testHostName, pageBuilder.GetWebPage().HostName);
        }

        [Test]
        public void AddLanguage()
        {
            //arrange
            var testLanguage = Language.NL;
            var pageBuilder = new WebPageBuilder();

            //act
            pageBuilder = pageBuilder.AddLanguage(testLanguage);
            
            //assert
            Assert.IsTrue(pageBuilder.AddLanguage(testLanguage) is WebPageBuilder);
            Assert.AreEqual(testLanguage, pageBuilder.GetWebPage().Language);
        }
        
        [Test]
        public void AddTestTitle()
        {
            //arrange
            var testTitle = "Test title";
            var pageBuilder = new WebPageBuilder();

            //act
            pageBuilder = pageBuilder.AddTitle(testTitle);
            
            //assert
            Assert.IsTrue(pageBuilder.AddTitle(testTitle) is WebPageBuilder);
            Assert.AreEqual(testTitle, pageBuilder.GetWebPage().Title);
        }
        
        [Test]
        public void AddWebsiteName()
        {
            //arrange
            var testName = "test.nl";
            var pageBuilder = new WebPageBuilder();

            //act
            pageBuilder = pageBuilder.AddWebsiteName(testName);
            
            //assert
            Assert.IsTrue(pageBuilder.AddWebsiteName(testName) is WebPageBuilder);
            Assert.AreEqual(testName, pageBuilder.GetWebPage().WebsiteName);
        }
        
        [Test]
        public void AddMainContent()
        {
            //arrange
            var testContent = "This is a test article";
            var pageBuilder = new WebPageBuilder();

            //act
            pageBuilder = pageBuilder.AddMainContent(testContent);
            
            //assert
            Assert.IsTrue(pageBuilder.AddMainContent(testContent) is WebPageBuilder);
            Assert.AreEqual(testContent, pageBuilder.GetWebPage().MainContent);
        }
        

        [Test]
        public void GetWebPage()
        {
            //arrange
            var testUrl = "test.com";
            var testDate = DateTime.Parse("7 september 2022");
            var testTitle = "test title";
            var pageBuilder = new WebPageBuilder();

            //act
            pageBuilder = pageBuilder.AddUrl(testUrl)
                .AddPublishingDate(testDate)
                .AddTitle(testTitle);

            //assert
            Assert.IsTrue(pageBuilder.GetWebPage() is WebPage);
            Assert.AreEqual(testUrl, pageBuilder.GetWebPage().Url);
            Assert.AreEqual(testDate, pageBuilder.GetWebPage().DatePosted);
            Assert.AreEqual(testTitle, pageBuilder.GetWebPage().Title);
        }
    }
}