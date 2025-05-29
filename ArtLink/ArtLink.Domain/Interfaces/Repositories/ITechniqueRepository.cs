using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Repositories;

/// <summary>
/// Defines the repository interface for managing techniques in the data access layer.
/// </summary>
public interface ITechniqueRepository
{
    /// <summary>
    /// Asynchronously retrieves all techniques.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a collection of all techniques.
    /// </returns>
    Task<IEnumerable<Technique>> GetAllAsync();

    /// <summary>
    /// Asynchronously adds a new technique.
    /// </summary>
    /// <param name="name">The name of the technique.</param>
    /// <param name="description">The description of the technique.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Guid> AddAsync(string name, string description);

    /// <summary>
    /// Asynchronously updates an existing technique.
    /// </summary>
    /// <param name="id">The unique identifier of the technique to update.</param>
    /// <param name="name">The new name of the technique.</param>
    /// <param name="description">The new description of the technique.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Guid id, string name, string description);

    /// <summary>
    /// Asynchronously deletes a technique by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the technique to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}
