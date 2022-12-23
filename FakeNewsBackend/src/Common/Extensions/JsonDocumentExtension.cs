using System.Text.Json;

namespace FakeNewsBackend.Common.Extensions;

public static class JsonDocumentExtension
{
    public static bool TryParse(string document, out JsonDocument val)
    {
        try
        {
            JsonDocument jsonDocument = JsonDocument.Parse(document);

            val = jsonDocument;
            return true;
        }
        catch (Exception e)
        {
            val = null;
            return false;
        }
    }
}