namespace FakeNewsBackend.Common.Extensions;

public static class LinqExtension
{
    public static IEnumerable<(Tsource item, int i)> WithIndex<Tsource>(this IEnumerable<Tsource> s)
    {
        return s.Select((item, i) => (item, i));
    }

    public static Tsource Sample<Tsource>(this IEnumerable<Tsource> source)
    {
        var count = source.Count();
        var r = new Random();
        var i = r.Next(count - 1);
        return source.WithIndex()
            .First(s => s.i == i).item;
    }
}