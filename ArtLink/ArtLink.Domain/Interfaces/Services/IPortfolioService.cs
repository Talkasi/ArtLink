using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Services;

public interface IPortfolioService
{
    /// <summary>
    /// Asynchronously retrieves a portfolio by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <see cref="Portfolio"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Portfolio?> GetPortfolioByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all portfolios associated with a specific artist.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist whose portfolios are to be retrieved.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a collection of <see cref="Portfolio"/> entities.
    /// </returns>
    Task<IEnumerable<Portfolio>> GetAllByArtistIdAsync(Guid artistId);

    /// <summary>
    /// Asynchronously adds a new portfolio for the specified artist.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist creating the portfolio.</param>
    /// <param name="title">The title of the portfolio.</param>
    /// <param name="techniqueId">The unique identifier of the technique associated with the portfolio.</param>
    /// <param name="description">An optional description of the portfolio.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Guid> AddPortfolioAsync(Guid artistId,
        string title,
        Guid techniqueId,
        string? description = null);

    /// <summary>
    /// Asynchronously updates an existing portfolio with new information.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio to update.</param>
    /// <param name="artistId">The unique identifier of the artist who owns the portfolio.</param>
    /// <param name="title">The new title of the portfolio.</param>
    /// <param name="techniqueId">The unique identifier of the new technique associated with the portfolio.</param>
    /// <param name="description">The new description of the portfolio (optional).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdatePortfolioAsync(Guid id,
        Guid artistId,
        string title,
        Guid techniqueId,
        string? description = null);

    /// <summary>
    /// Asynchronously deletes a portfolio by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeletePortfolioAsync(Guid id);
}
