namespace FakeNewsBackend.Common.exceptions;

public class SimilarityException : Exception
{
    public SimilarityException(string message) : base(message) {}
}