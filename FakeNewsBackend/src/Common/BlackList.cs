using FakeNewsBackend.Common.Types;

namespace FakeNewsBackend.Common;

public static class BlackList
{
    public static string[] blackList =
    {
        "tag",
        "category",
        "categories",
        "feed",
        "author",
        "forum",
        "agenda",
        "profiel",
        "categorie",
        "taxonomies",
        "user",
        // "page",
        "comment",
        ".pdf",
    };

    public static string[] blackListForGenerating =
    {
        ".pdf",
        ".css",
        ".js",
        ".jpg",
        ".jpeg",
        ".JPG",
        ".png",
        ".gif",
        ".PNG",
        ".svg",
        ".ico",
        "wp-content",
        "wp-json",
        "wp-",
        "comment",
        "#",
        "search",
        "banners",
        "tmpl",
        "images",
        "/feed/",
        "/component/",
        "/user/",
        "/rss/",
        "/linkuser/",
        "/pictures/",
    };
    public static string[] BlackListForSearching = {
        ".twitter.",
        ".facebook.",
        ".twimmer.",
        ".youtube.",
        ".vk.",
        ".wikipedia",
    };
    
    /// <summary>
    /// Checks if the given <paramref name="link"/> is in the black list or not.
    /// </summary>
    /// <param name="link">Url to check.</param>
    /// <param name="typeOfBlackList">The type of blacklist to use.</param>
    /// <returns>Whether the <paramref name="link"/> is in the blacklist.</returns>
    public static bool LinkIsInBlackList(string link, Blacklist typeOfBlackList = Blacklist.Standard)
    {
        var list = typeOfBlackList switch
        {
            Blacklist.Generating => blackListForGenerating,
            Blacklist.Search => BlackListForSearching,
            _ => blackList
        };
        
        foreach (var part in list)
            if (link.Contains(part)) 
                return true;
        return false;
    }
}