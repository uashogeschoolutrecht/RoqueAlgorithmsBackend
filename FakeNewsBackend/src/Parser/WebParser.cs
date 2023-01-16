using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using FakeNewsBackend.Common;
using FakeNewsBackend.Common.debug;
using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Common.Types;
using FakeNewsBackend.Domain.DTO;
using HtmlAgilityPack;

namespace FakeNewsBackend.Parser
{
    public class WebParser : IDisposable
    {
        private HtmlDocument _htmlPage;
        private bool isDisposed = false;
        public WebParser(string pageContent)
        {
            if (pageContent == "")
                throw new HtmlDocumentException("Invalid html was given");
            _htmlPage = new HtmlDocument();
            _htmlPage.LoadHtml(pageContent);
            FilterPage();
        }

        #region public methods

        /// <summary>
        /// Tries to get the <see cref="Language"/> of the page out of the document.
        /// </summary>
        /// <returns>The found <see cref="Language"/></returns>
        public Language GetLanguage()
        {
            var languageValue = _htmlPage.DocumentNode.SelectSingleNode("//html")?.Attributes
                .Select(attribute => attribute.Name == "lang" ? attribute.Value : null)
                .FirstOrDefault(value => value != null, "");
            return Mapper.GetLanguage(languageValue);
        }
        /// <summary>
        /// Tries to get the article from the document.
        /// </summary>
        /// <returns>The article if found, if not it returns: "Not Found".</returns>
        public string GetMainArticle()
        {
            var node = FindArticleTag();
            if (node == null) return "Not Found";
            return WebUtility.HtmlDecode(FilterArticle(node).InnerText)
                .Trim()
                .Replace("  ","") 
                .Replace("\t","")
                .Replace("\r","")
                .Replace("\n\n"," ")
                .Replace(".",". ");
        }
        
        /// <summary>
        /// Tries to get the title of the article from the document.
        /// </summary>
        /// <returns>The Title if found, if not it returns: "Not Found".</returns>
        public string GetTitle()
        {
            var foundTitle = FindTitle()?.InnerText;
            if (foundTitle == null) return "Not Found";
            return WebUtility.HtmlDecode(foundTitle.Trim());
        }

        /// <summary>
        /// Tries to get the publishing date from the document.
        /// </summary>
        /// <returns>The date if found, if not it returns: the minimal value.</returns>
        public DateTime GetDate()
        {
            try{
                var published =_htmlPage.DocumentNode
                    .SelectSingleNode("//meta[@property=\"article:published_time\"]")?
                    .GetAttributeValue("content","");
                if (published != null)
                {
                    var date = DateTime.MinValue;
                    if (DateTime.TryParse(published,out date))
                        return date.ToUniversalTime();
                }
                var foundDate = FindDate();
                if(foundDate?.GetAttributeValue("datetime",null) != null){
                    return DateTime.Parse(foundDate?.GetAttributeValue("datetime",null)!)
                        .ToUniversalTime();
                }
                var dt = DateTime.MinValue;
                if (foundDate != null && foundDate.InnerText.Length <= 17)
                {
                    DateUtils.ParseDates(foundDate.InnerText, out dt);
                    return dt.ToUniversalTime();
                }
                var page = _htmlPage.DocumentNode.SelectSingleNode("//body")?.InnerText;
                if (page == null)
                    return dt.ToUniversalTime();
                var matches = Regex.Matches(page, "[0-9]{1,2}[-/,.][0-9]{1,2}[-/,.][0-9]{4}");
                if (matches.Count <= 0)
                    return dt.ToUniversalTime();
                var mostRecentDate = matches.Select(match =>
                {
                    DateUtils.ParseDates(match.Value, out DateTime loopdt);
                    return loopdt.ToUniversalTime();
                }).Max();
                return mostRecentDate.ToUniversalTime();
            }
            catch (Exception e) 
            {
                return DateTime.MinValue.ToUniversalTime();
            }
        }

        /// <summary>
        /// Tries to get the website name from the document.
        /// </summary>
        /// <param name="backupUri">the url to fallback on.</param>
        /// <returns>The name of the website if found, if not it returns: "Not Found"</returns>
        public string GetWebsiteName(Uri? backupUri)
        {
            var siteName = _htmlPage.DocumentNode
                .SelectSingleNode("//meta[@property=\"og:site_name\"]")?
                .GetAttributeValue("content","");

            return siteName ?? backupUri?.Host ?? "Not Found";
        }

        /// <summary>
        /// Check if there is only one article on the document.
        /// </summary>
        /// <returns>Whether there is only one article.</returns>
        public bool HasOnlyOneArticle()
        {
            return GetArticleCount() == 1;
        }
        
        /// <summary>
        /// Gets all links in a `href` attribute on the document.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all the links on the page as a <see cref="UrlItemDTO"/>.</returns>
        public IEnumerable<UrlItemDTO> GetAllLinksOnPage()
        {
            var nodes = _htmlPage.DocumentNode.SelectNodes("//*[@href]");

            if (nodes == null || nodes.Count == 0)
                return new List<UrlItemDTO>();
            return nodes.Select(node =>  new UrlItemDTO
            {
                url = node.GetAttributeValue("href", ""),
                lastmod = DateTime.MinValue.ToUniversalTime()
            });
        }
        
        /// <summary>
        /// Tries to find the date.
        /// </summary>
        /// <returns>A <see cref="HtmlNode"/> if found, if not: null</returns>
        public HtmlNode FindDate()
        {
            var tagList = new List<string>
            {
               "//time",
                "//*[@datetime]",
                "//*[contains(@class,'date')]",
                "//*[contains(@class,'publish')]"
            };
            foreach (var tag in tagList)
            {
                var node = _htmlPage.DocumentNode.SelectSingleNode(tag);
                if (node != null)
                    return node;
            }
            return null;
        }
        
        /// <summary>
        /// Tries to find the title.
        /// </summary>
        /// <returns>A <see cref="HtmlNode"/> if found, if not: null</returns>
        public HtmlNode FindTitle()
        {
            //Titel moet nog verbeterd worden
            // nieuwsblik doet kut, heeft een 2de h1 die ik niet moet hebben 
            var tagsList = new List<string>
            {
                "//*[substring-after(name(), 'h') > 0][contains(@class,'title')]",
                "//*[contains(@class,'post-title')]",
                "//h1",
                "//h2",
                "//*[contains(@id,'title')]",
                "//*[contains(@class,'title')]",
                };
            foreach (var tag in tagsList)
            {
                var nodes = _htmlPage.DocumentNode.SelectNodes(tag);
                if (nodes == null)
                    continue;
                foreach (var node in nodes)
                {
                    if (!string.IsNullOrWhiteSpace(node.InnerText) 
                        && !node.GetAttributeValue("class","").Contains("hide"))
                        return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to find the article.
        /// </summary>
        /// <returns>A <see cref="HtmlNode"/> if found, if not: null</returns>
        public HtmlNode? FindArticleTag()
        {
            HtmlNode longestInnerText = null;
            var tagsList = new List<string>
            {
                "//article",
                "//div[contains(@id,'content')]",
                "//div[contains(@id,'article')]",
                "//div[contains(@id,'post')]",
                "//div[contains(@class,'article')]",
                "//div[contains(@class,'content')]",
                "//div[contains(@class,'article-container')]",
                "//*[contains(@id,'article')]",
                "//*[contains(@id,'post')]",
            };
            foreach (var tag in tagsList)
            {
                var nodes = _htmlPage.DocumentNode.SelectNodes(tag);
                if (nodes == null)
                    continue;
                foreach (var htmlNode in nodes)
                {
                    if (longestInnerText == null)
                        longestInnerText = htmlNode;
                    
                    if (longestInnerText.InnerText.Length < htmlNode.InnerText.Length)
                        longestInnerText = htmlNode;
                }

                if (longestInnerText != null)
                    return longestInnerText;
            }
            return longestInnerText;
        }
        /// <summary>
        /// Gets the amount of nodes there are in the document of the first tag which gives result.
        /// </summary>
        /// <returns>The amount of article nodes there are</returns>
        public int GetArticleCount()
        {
            var tagsList = new List<string>
            {
                "//article",
                "//div[contains(@id,'article')]",
                "//div[contains(@id,'content')]",
                "//div[contains(@class,'article')]",
                "//div[contains(@class,'content')]"
            };
            foreach (var tag in tagsList)
            {
                var node = _htmlPage.DocumentNode.SelectNodes(tag);
                if(node != null)
                    return node.Count;
            }
            return 0;
        }
        /// <summary>
        /// Filters the article
        /// </summary>
        /// <param name="nodeToFilter">The node to filter.</param>
        /// <returns>The filtered <see cref="HtmlNode"/></returns>
        public static HtmlNode FilterArticle(HtmlNode nodeToFilter)
        {
            if(nodeToFilter == null || nodeToFilter.OuterHtml == "") return null;
            var filter = new List<string>
            {
                "//script",
                "//img",
                "//svg",
                "//video",
                "//audio",
                "//iframe",
                "//form",
                "//aside",
                "//nav",
                "//time",
                "//div[contains(@class,'code-block')]",
                "//div[contains(@class,'caption')]",
                "//div[contains(@class,'agreement')]",
                "//div[contains(@class,'info')]",
                "//div[contains(@class,'tags')]",
                "//div[contains(@class,'share')]",
                "//div[contains(@class,'rating')]",
                "//div[contains(@class,'comment')]",
                "//div[contains(@class,'related')]",
                "//div[contains(@class,'copyright')]",
                "//div[contains(@class,'ad') and not(contains(@class, 'header'))]",
                "//div[contains(@class,'meta')]",
                "//div[contains(@class,'image')]",
                "//div[contains(@class,'twitter')]",
                "//div[contains(@class,'facebook')]",
                "//div[contains(@class,'youtube')]",
                "//div[contains(@class,'sidebar')]",
                "//*[contains(@class,'code') and not(contains(@class, 'content'))]",
                "//*[contains(@class,'code-block')]",
                "//*[contains(@class,'info')]",
                "//*[contains(@class,'audio')]",
                "//*[contains(@class,'tags')]",
                "//footer",
                "//*[substring-after(name(), 'h') > 0][contains(@class,'title')]",
                "//*[contains(@class,'post-title')]",
                "//h1",
            };
            foreach (var filterItem in filter)
            {
                var nodes = nodeToFilter.SelectNodes(filterItem);
                if(nodes == null) continue;
                foreach (var node in nodes)
                {
                    node.Remove();
                    node.RemoveAll();
                }
            }
            return nodeToFilter;
        }

        private void FilterPage()
        {
            var tagsList = new List<string>
            {
                "//*[contains(@class,'modal') and not(self::body)]",
            };
            foreach (var tag in tagsList)
            {
                var nodes = _htmlPage.DocumentNode.SelectNodes(tag);
                if (nodes == null)
                    continue;
                foreach (var node in nodes)
                {
                    node.Remove();
                    node.RemoveAll();
                }
            }
        }

        public static string GetFirstSentence(string article)
        {
            var charList = new List<char>()
            {
                '.',
                '!',
                '?'
            };
            string firstFoundSentence = default;
            foreach (var c in charList)
            {
                var tempSentence = article.Split(c)[0];
                if (firstFoundSentence == default)
                    firstFoundSentence = tempSentence;
                if (firstFoundSentence.Length > tempSentence.Length)
                    firstFoundSentence = tempSentence;
            }

            return firstFoundSentence;
        }

        #endregion

        #region memoryManagement
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _htmlPage = null;
                // Code to dispose the managed resources of the class
            }
            // Code to dispose the un-managed resources of the class
            isDisposed = true;
        }
        #endregion
        
    }
}
