using FakeNewsBackend.Common.Types;

namespace FakeNewsBackend.Common
{
    public static class Mapper
    {
        public static Language GetLanguage(string? languageInStringFormat)
        {
            if (string.IsNullOrEmpty(languageInStringFormat))
                return Language.UNKNOWN;
            if (languageInStringFormat.ToLower().Contains("en"))
                return Language.EN;
            if (languageInStringFormat.ToLower().Contains("nl"))
                return Language.NL;
            return Language.UNKNOWN;
        }

        public static string LanguageToString(Language language) => language switch
        {
            Language.EN => "english",
            Language.NL => "dutch",
            Language.UNKNOWN => "unknown",
            _ => "unknown"
        };
    }
}
