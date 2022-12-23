using FakeNewsBackend.Domain;

namespace FakeNewsBackend.Service.Interface;

public interface IRobotRulesService : IService<RobotRules>
{
    /// <summary>
    /// Retrieve a <see cref="RobotRules"/> object with the given <see cref="Website"/> object.
    /// </summary>
    /// <param name="website">The <see cref="Website"/> of which the <see cref="RobotRules"/> needs to be retrieved.</param>
    /// <returns>A <see cref="RobotRules"/> object found with the given <see cref="Website"/></returns>
    public RobotRules GetRulesOfWebsite(Website website);

    /// <summary>
    /// Checks if a <see cref="RobotRules"/> with the given <paramref name="id"/> exists.
    /// </summary>
    /// <param name="id">id of the <see cref="Website"/>.</param>
    /// <returns>A <see cref="bool"/> whether a <see cref="RobotRules"/> object with the given <paramref name="id"/> was found.</returns>
    public bool ExistsWithWebsiteId(int id);
}