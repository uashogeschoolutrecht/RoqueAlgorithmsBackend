namespace FakeNewsBackend.Service.Interface;

/// <summary>
/// Represent the very basic behavior of services.
/// </summary>
/// <typeparam name="T">the type of object the service needs to use.</typeparam>
public interface IService<T>
{
    /// <summary>
    /// Add an object to the database.
    /// </summary>
    /// <param name="obj">The object to add.</param>
    public void Add(T obj);
    
    /// <summary>
    /// Update an object in the database.
    /// </summary>
    /// <param name="obj">The object to update.</param>
    public void Update(T obj);
    
    /// <summary>
    /// Delete an object from the database.
    /// </summary>
    /// <param name="obj">The object to delete.</param>
    public void Delete(T obj);
}