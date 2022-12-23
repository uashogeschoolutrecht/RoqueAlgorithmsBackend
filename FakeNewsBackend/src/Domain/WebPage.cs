using FakeNewsBackend.Common.debug;
using FakeNewsBackend.Common.Types;

namespace FakeNewsBackend.Domain
{
    public class WebPage
    {
        public string Title;
        public string Url;
        public string WebsiteName;
        public int WebsiteId;
        public string HostName;
        public DateTime DatePosted { get; set; }
        public Language Language;
        public string MainContent;

        public string ToString()
        {
            return $" title={Title},\n Url={Url},\n WebsiteName={WebsiteName},\n HostName={HostName},\n datePosted={DatePosted.ToString()},\n language={Language},\n MainContent={StringLiteral.ToLiteral(MainContent)},\n";
        }

        protected bool Equals(WebPage other)
        {
            return Title == other.Title && 
                   Url == other.Url && 
                   WebsiteName == other.WebsiteName && 
                   HostName == other.HostName && 
                   Language == other.Language && 
                   MainContent == other.MainContent && 
                   Nullable.Equals(DatePosted, other.DatePosted);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WebPage)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Url, WebsiteName, HostName, (int)Language, MainContent, DatePosted);
        }
    }
}
