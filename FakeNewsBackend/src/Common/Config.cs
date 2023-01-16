using FakeNewsBackend.Common.exceptions;
using FakeNewsBackend.Common.Extensions;

using Microsoft.Extensions.Configuration;

namespace FakeNewsBackend.Common
{
    public static class Config
    {
        public static string GetConnectionString(string attributeName)
        {
            var filepath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName +
                           "/solutionsettings.json";
            if(!File.Exists(filepath))
                filepath = Directory.GetParent(Environment.CurrentDirectory).FullName +
                           "/solutionsettings.json";
            var filepathBackup = Path.GetFullPath("solutionsettings.json");
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile(File.Exists(filepath) ? filepath : filepathBackup, false, true);
            IConfigurationRoot root = builder.Build();
            return root[attributeName];

        }

        public static IEnumerable<string> GetDataBaseSeed(string attributeName)
        {
            var filepath = Directory.GetParent(Environment.CurrentDirectory).FullName +
                           "/FakeNewsBackend/Resources/seeddatabase.json";



            var data = File.ReadAllText(filepath);
            if (!JsonDocumentExtension.TryParse(data, out var document))
                throw new JsonElementParseException("Seed database Failed");
            if (!document.RootElement.TryGetProperty(attributeName, out var el))
                throw new JsonElementParseException("seeddatabase.json is not setup correctly");
            return el.EnumerateArray()
                .Select(s => s.GetRawText());
        }

    }
}
