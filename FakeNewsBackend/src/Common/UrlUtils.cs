using System.Text.RegularExpressions;

namespace FakeNewsBackend.Common;

public class UrlUtils
{
    private static Regex year = new ("/[0-9]{4}/");
    public static bool UrlHasDate(string url)
    {
        return year.IsMatch(url);
    }

    public static int GetYearOutOfUrl(string url)
    {
        var str = year.Match(url);
        return Int32.Parse(str.Value.Replace("/", ""));
    }
    
    public static string SanetizeUrl(string url)
    {
        var result = url;
        if (result.EndsWith('/'))
            result = result.Remove(result.Length - 1);
        if (!result.Contains("www.") && result.Count(c => c == '.') >= 2)
            result = result.Insert(
                result.IndexOf("://", StringComparison.Ordinal) + 3, "www.");
        return result.Replace("\"","");
    }

    public static string RemoveProtocol(string url)
    {
        return url.Substring(url.IndexOf("://", StringComparison.Ordinal) + 3);
    }
}