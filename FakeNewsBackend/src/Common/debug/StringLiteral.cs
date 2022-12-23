
namespace FakeNewsBackend.Common.debug;

public static class StringLiteral
{
    // Just for debugging
    public static string ToLiteral(string valueTextForCompiler)
    {
        return Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(valueTextForCompiler, false);

    }
}