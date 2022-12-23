using FakeNewsBackend.Domain.DTO;

namespace FakeNewsBackend.Common;

public class DistinctUrlComparer : IEqualityComparer<UrlItemDTO>
{
    public bool Equals(UrlItemDTO x, UrlItemDTO y)
    {
        return x.url == y.url;
    }

    public int GetHashCode(UrlItemDTO obj)
    {
        return obj.url.GetHashCode();
    }
    
}