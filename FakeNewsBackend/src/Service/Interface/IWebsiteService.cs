using FakeNewsBackend.Domain;

namespace FakeNewsBackend.Service.Interface;

public interface IWebsiteService : IService<Website>
{
    /// <summary>
    /// Save the <see cref="WebsiteProgress"/> object in the database.
    /// </summary>
    /// <param name="progress">Object to save.</param>
    public void SaveProgress(WebsiteProgress progress);

    /// <summary>
    /// Update the <see cref="WebsiteProgress"/> object in the database.
    /// </summary>
    /// <param name="progress">Object to update.</param>
    public void UpdateProgress(WebsiteProgress progress);

    /// <summary>
    /// Checks if a <see cref="Website"/> with the given <paramref name="url"/> exists.
    /// </summary>
    /// <param name="url">url of the <see cref="Website"/>.</param>
    /// <returns>A <see cref="bool"/> whether a <see cref="Website"/> with the given <paramref name="url"/> was found.</returns>
    public bool WebsiteExistsWithUrl(string url);
    
    /// <summary>
    /// Retrieve a <see cref="Website"/> with the given <paramref name="url"/>.
    /// </summary>
    /// <param name="url">Url of the <see cref="Website"/>.</param>
    /// <returns>The <see cref="Website"/> which has the <paramref name="url"/>.</returns>
    public Website GetWebSiteByUrl(string url);
    
    /// <summary>
    /// Retrieve the progress of a <see cref="Website"/> with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The id of the <see cref="Website"/>.</param>
    /// <returns>A <see cref="WebsiteProgress"/> which has the <paramref name="id"/>.</returns>
    public WebsiteProgress GetProgressById(int id);

    /// <summary>
    /// Get All websites from database
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> with all websites</returns>
    public IEnumerable<Website> GetAll();
    
    /// <summary>
    /// Get a website from the database that is not set up.
    /// </summary>
    /// <returns>A <see cref="Website"/> to setup.</returns>
    public Website GetWebsiteToSetup();
    
    /// <summary>
    /// Checks if the database contains a <see cref="Website"/> which is not set up.
    /// </summary>
    /// <returns>A <see cref="bool"/> whether the database contains a <see cref="Website"/> which is not set up.</returns>
    public bool HasWebsiteToSetup();

    /// <summary>
    /// Checks if the database contains a <see cref="Website"/> which is not completed.
    /// </summary>
    /// <returns>A <see cref="bool"/> whether the database contains an uncompleted <see cref="Website"/>.</returns>
    public bool HasNextWebsite();

    /// <summary>
    /// Checks if the database contains websites.
    /// </summary>
    /// <returns>A <see cref="bool"/> whether the database contains a <see cref="Website"/>.</returns>
    public bool HasWebsites();

    /// <summary>
    /// Retrieve uncompleted <see cref="Website"/> which are setup from database.
    /// </summary>
    /// <returns>A uncompleted <see cref="Website"/>.</returns>
    public Website GetUncompletedWebsite();
}