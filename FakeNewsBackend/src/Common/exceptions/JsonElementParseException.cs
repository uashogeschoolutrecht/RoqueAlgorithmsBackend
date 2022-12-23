namespace FakeNewsBackend.Common.exceptions;

public class JsonElementParseException : Exception
{
    public JsonElementParseException(string? message) : base(message) { }
}