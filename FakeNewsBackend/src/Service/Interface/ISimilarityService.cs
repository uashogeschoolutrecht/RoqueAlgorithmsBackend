using FakeNewsBackend.Domain;

namespace FakeNewsBackend.Service.Interface;

public interface ISimilarityService : IService<Similarity>
{
    /// <summary>
    /// Retrieve a <see cref="Similarity"/> object.
    /// </summary>
    /// <param name="firstLink">The url of one of the links</param>
    /// <param name="secondLink">The url of one of the links</param>
    /// <returns>A <see cref="Similarity"/> object found with the given parameters</returns>
    public Similarity GetSimilarityByLinks(string firstLink, string secondLink);
    
    /// <summary>
    /// Retrieve all <see cref="Similarity"/> from two given website id.
    /// </summary>
    /// <param name="webSite1Id">The id of one of the websites</param>
    /// <param name="webSite2Id">The id of one of the websites</param>
    /// <returns>A <see cref="IEnumerable{Similarity}"/> that contains <see cref="Similarity"/>.</returns>
    public IEnumerable<Similarity> GetSimilaritiesByWebsitesId(int webSite1Id, int webSite2Id);
    
    /// <summary>
    /// Retrieve all <see cref="Similarity"/> from two given website id which have the same order as the given ids
    /// </summary>
    /// <param name="webSite1Id">The id of one of the websites</param>
    /// <param name="webSite2Id">The id of one of the websites</param>
    /// <returns>A <see cref="IEnumerable{Similarity}"/> that contains <see cref="Similarity"/>.</returns>
    public IEnumerable<Similarity> GetSimilaritiesByWebsitesIdInOrder(int webSite1Id, int webSite2Id);

    /// <summary>
    /// Retrieve all <see cref="Similarity"/> from the database where one or two of the article have 'no' date.
    /// </summary>
    /// <returns>A <see cref="IEnumerable{Similarity}"/> that contains <see cref="Similarity"/>.</returns>
    public IEnumerable<Similarity> GetSimilaritiesWithUncertainUrls();

    /// <summary>
    /// Retrieve all <see cref="Similarity"/> from one website id.
    /// </summary>
    /// <param name="id">The id of the website</param>
    /// <returns>A <see cref="IEnumerable{Similarity}"/> that contains <see cref="Similarity"/>.</returns>
    public IEnumerable<Similarity> GetSimilaritiesByWebsiteId(int id);
    
    /// <summary>
    /// Retrieve all <see cref="Similarity"/> from the database.
    /// </summary>
    /// <returns>A <see cref="IEnumerable{Similarity}"/> that contains <see cref="Similarity"/>.</returns>
    public IEnumerable<Similarity> GetAll();
    
    /// <summary>
    /// Checks if a <see cref="Similarity"/> with the two given <see cref="WebPage"/>s exists.
    /// </summary>
    /// <param name="originalPage">One of the webpages.</param>
    /// <param name="foundPage">One of the webpages.</param>
    /// <returns>A <see cref="bool"/> whether a <see cref="Similarity"/> object with the given parameters was found.</returns>
    public bool Exists(WebPage originalPage, WebPage foundPage);

    /// <summary>
    /// Updates <see cref="Similarity"/> after the combined keys have been switched.
    /// </summary>
    /// <param name="oldSim">the <see cref="Similarity"/> that is in the database</param>
    /// <param name="newSim">the <see cref="Similarity"/> that has been swapped</param>
    public void UpdateSimilarityAfterSwap(Similarity oldSim, Similarity newSim);
}