using FakeNewsBackend.Common;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Domain;

namespace FakeNewsBackend.Builder
{
    public class WebPageBuilder
    {
        private string Title;
        private string Url;
        private string WebsiteName;
        private string HostName;
        private DateTime DatePosted;
        private Language Language;
        private string MainContent;

        public WebPageBuilder AddPublishingDate(DateTime date)
        {
            DatePosted = date;
            return this;
        }
        public WebPageBuilder AddMainContent(string content)
        {
            MainContent = content;
            return this;
        }
        public WebPageBuilder AddTitle(string title)
        {
            Title = title;
            return this;
        }
        public WebPageBuilder AddHostName(string name)
        {
            HostName = name;
            return this;
        }
        public WebPageBuilder AddWebsiteName(string name)
        {
            WebsiteName = name;
            return this;
        }
        public WebPageBuilder AddLanguage(Language language)
        {
            Language = language;
            return this;
        }
        
        public WebPageBuilder AddUrl(string url)
        {
            Url = url;
            return this;
        }
        public WebPage GetWebPage()
        {
            return new WebPage
            {
                Url = Url,
                Title = Title,
                WebsiteName = WebsiteName,
                HostName = HostName,
                DatePosted = DatePosted,
                Language = Language,
                MainContent = MainContent,
            };
        }
    }
}
