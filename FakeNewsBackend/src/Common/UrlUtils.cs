using System.Text.RegularExpressions;

namespace FakeNewsBackend.Common;

public class UrlUtils
{
    private static List<Regex> dateWithOnlyYearList = new List<Regex>() {
        new (@"\/(20([0-1][0-9]|2[0-3]))\/")};
    private static List<Regex> dateWithOnlyMonthList = new List<Regex>() {
        new(@"\/(20([0-1][0-9]|2[0-3])\/(0[0-9]|1[0-2]))\/")};
    private static List<Regex> dateList = new List<Regex>(){
        new(@"\/(20([0-1][0-9]|2[0-3])\/(0[0-9]|1[0-2])\/([0-2][0-9]|3[0-1]))\/"),
        new(@"\/([0-9]{4}\/[0-9]{2}\/[0-9]{2})\/") ,
        new(@"\/(\d{4}\/\d{1,2}\/\d{1,2})\/"),
        new(@"\/(\d{4}\/\d{2}\/\d{2})\/") ,
        new(@"/(\d{4})/(\d{2})/(\d{2})/"),
        new("/[0-9]{4}/[0-9]{2}/[0-9]{2}/"),
        new(@"/[0-9]{4}\/[0-9]{2}\/[0-9]{2}/"),
        new(@"\/\d{4}\/\d{2}\/\d{2}\/"),
    };

    public static bool UrlHasDate(string url)
    {
        foreach (Regex regex in dateWithOnlyYearList)
        {
            if (regex.IsMatch(url)) return true;
        }
        return false;
    }
    public static bool UrlHasTotalDate(string url)
    {
        foreach (Regex regex in dateList)
        {
            if(regex.IsMatch(url))
            {
                Console.WriteLine($"url == {url}");
                return true;
            }
                
        }
        return false;
    }
    public static bool UrlHasMonth(string url)
    {
        foreach (Regex regex in dateWithOnlyMonthList)
        {
            if (regex.IsMatch(url)) return true;
        }
        return false;
    }
    public static int GetYearOutOfUrl(string url)
    {
        string result = "";
        foreach(Regex regex in dateWithOnlyYearList)
        {
            if (regex.IsMatch(url))
            {
                result = regex.Match(url).ToString();
                break;
            }
        }
        return Int32.Parse(result.Replace("/", ""));
    }
    public static DateTime GetYearOutOfUrlAsDate(string url)
    {
        Console.WriteLine(url);
        string result = "";
        foreach (Regex regex in dateWithOnlyYearList)
        {
            if (regex.IsMatch(url))
            {
                result = regex.Match(url).ToString();
                break;
            }
        }
        Console.WriteLine(result);

        var stripped = result.Remove(0,1);
        return DateTime.Parse(stripped + "01/02").ToUniversalTime();
    }
    public static DateTime GetDateOutOfUrl(string url)
    {
        Console.WriteLine(url);
        string result = "";
        foreach (Regex regex in dateList)
        {
            if (regex.IsMatch(url))
            {
                result = regex.Match(url).ToString();
                break;
            }
        }
        if (!result.StartsWith("/"))
            return DateTime.Parse(result).ToUniversalTime();
        var stripped = result.Substring(1, result.Length - 2);
        return DateTime.Parse(stripped).ToUniversalTime();
    }
    public static DateTime GetMonthOutOfUrl(string url)
    {
        string result = "";
        foreach (Regex regex in dateWithOnlyMonthList)
        {
            if (regex.IsMatch(url))
            {
                result = regex.Match(url).ToString();
                break;
            }
        }
        var stripped = result.Substring(1, result.Length - 2);
        return DateTime.Parse(stripped + "/02").ToUniversalTime();
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