using System.Text.RegularExpressions;

namespace FakeNewsBackend.Common;

public class UrlUtils
{
    private static Regex dateWithOnlyYear = new ("/[0-9]{4}/");
    private static Regex dateWithOnlyMonth = new("/[0-9]{4}/[0-9]{2}/");
    private static Regex date = new("/[0-9]{4}/[0-9]{2}/[0-9]{2}/");

    public static bool UrlHasDate(string url)
    {
        return dateWithOnlyYear.IsMatch(url);
    }
    public static bool UrlHasTotalDate(string url)
    {
        return date.IsMatch(url);
    }
    public static bool UrlHasMonth(string url)
    {
        return dateWithOnlyMonth.IsMatch(url);
    }
    public static int GetYearOutOfUrl(string url)
    {
        var str = dateWithOnlyYear.Match(url);
        return Int32.Parse(str.Value.Replace("/", ""));
    }
    public static DateTime GetDateOutOfUrl(string url)
    {
        var str = date.Match(url);
        var stripped = str.Value.Replace("/", "-").Substring(1, str.Length - 2);
        return DateTime.Parse(stripped).ToUniversalTime();
    }
    public static DateTime GetMonthOutOfUrl(string url)
    {
        var str = dateWithOnlyMonth.Match(url);
        var stripped = str.Value.Replace("/", "-").Substring(1, str.Length - 2);
        return DateTime.Parse(stripped + "-01").ToUniversalTime();
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