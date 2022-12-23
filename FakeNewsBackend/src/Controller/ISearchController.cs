using FakeNewsBackend.Domain.DTO;

namespace FakeNewsBackend.Controller;

public interface ISearchController
{
    /// <summary>
    /// Uses a search API to search for new articles
    /// </summary>
    /// <param name="title">The title to search</param>
    /// <param name="originalSite">Url from the site to exclude from the results</param>
    /// <returns><see cref="Task"/> with an <see cref="IEnumerable{T}"/>
    /// containing articles found as <see cref="UrlItemDTO"/>.</returns>
    public Task<IEnumerable<UrlItemDTO>> SearchTitle(string title, string originalSite);
}