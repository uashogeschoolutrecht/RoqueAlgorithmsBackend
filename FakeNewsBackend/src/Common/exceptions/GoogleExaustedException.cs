namespace FakeNewsBackend.Common.exceptions;

public class GoogleExhaustedException : Exception
{
    public GoogleExhaustedException(string message) : base(message) { }

}