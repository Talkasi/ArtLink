using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Repositories;

public interface IPortfolioRepository
{
    /// <summary>
    /// Asynchronously retrieves a portfolio by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the portfolio if found; otherwise, it returns null.
    /// </returns>
    Task<Portfolio?> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all portfolios associated with a specific artist.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist whose portfolios are to be retrieved.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of portfolios associated with the specified artist.
    /// </returns>
    Task<IEnumerable<Portfolio>> GetAllByArtistIdAsync(Guid artistId);

    /// <summary>
    /// Asynchronously adds a new portfolio to the repository.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist to whom the portfolio belongs.</param>
    /// <param name="title">The title of the portfolio.</param>
    /// <param name="techniqueId">The unique identifier of the technique used in the portfolio.</param>
    /// <param name="description">A description of the portfolio.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Guid artistId,
        string title,
        Guid techniqueId,
        string? description = null);

    /// <summary>
    /// Asynchronously updates an existing portfolio in the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio to update.</param>
    /// <param name="artistId">The unique identifier of the artist to whom the portfolio belongs.</param>
    /// <param name="title">The updated title of the portfolio.</param>
    /// <param name="techniqueId">The unique identifier of the updated technique used in the portfolio.</param>
    /// <param name="description">The updated description of the portfolio.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Guid id,
        Guid artistId,
        string title,
        Guid techniqueId,
        string? description = null);

    /// <summary>
    /// Asynchronously deletes a portfolio by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}
