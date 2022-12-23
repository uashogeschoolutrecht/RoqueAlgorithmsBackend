using System.Globalization;

namespace FakeNewsBackend.Common;

public class DateUtils
{
    public static void ParseDates(string dateText, out DateTime dt)
    {
        var languages = new[] { "nl-NL", "en-US" };
        foreach (var lang in languages)
        {
            if (!DateTime.TryParse(dateText, new CultureInfo(lang, false), 
                    DateTimeStyles.None, out dt))
                continue;
            return;
        }
        dt = DateTime.MinValue;
    }
}