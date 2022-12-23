using System.Xml;
using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Domain.DTO;

namespace FakeNewsBackend.Parser
{
    public class XmlParser
    {
        private XmlDocument document;
        public void AddDocument(string document)
        {
            if(document == "") 
                throw new XmlDocumentException("No Document was given");
            this.document = new XmlDocument();
            this.document.LoadXml(document);
        }

        public IEnumerable<UrlItemDTO> ParseXml()
        {
            if (document == null)
                throw new XmlDocumentException("No Document was given");


            var xmlList = document.GetElementsByTagName("url");

            if (xmlList == null || xmlList.Count == 0)
                xmlList = document.GetElementsByTagName("sitemap");
            
            foreach (XmlNode node in xmlList)
            {
                if (node["loc"] == null)
                    continue;
                if (node["lastmod"] != null)
                    yield return new UrlItemDTO
                    {
                        url = node["loc"]?.InnerText,
                        lastmod = DateTime.Parse(node["lastmod"]?.InnerText).ToUniversalTime()
                    };
                else
                    yield return new UrlItemDTO
                    {
                        url = node["loc"]?.InnerText,
                        lastmod = DateTime.MinValue.ToUniversalTime()
                    };
            }
        }

        public XmlType GetXmlType()
        {
            if (document == null) 
                throw new XmlDocumentException("No Document was given");
            
            var links = ParseXml().ToList();
            if (links.All(item => item.url.Contains(".xml"))) 
                return XmlType.SITEMAP_SITEMAP;
            if (links.All(item => item.url.Contains("tag") || item.url.Contains("tags")))
                return XmlType.SITEMAP_TAG;
            if (links.All(item => item.url.Contains("category") || item.url.Contains("categories")))
                return XmlType.SITEMAP_CATEGORIES;
            if (links.All(item => item.url.Contains("author")))
                return XmlType.SITEMAP_AUTHOR;
            return XmlType.SITEMAP_ARTICLES;
        }
    }
}   
