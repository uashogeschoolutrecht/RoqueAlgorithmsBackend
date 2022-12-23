namespace FakeNewsBackend.Common.exceptions;

public class JsonElementNotFoundException : Exception
{
    public JsonElementNotFoundException(string message) : base(message) { }
}