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
    /// The task result contains the portfolio if found; otherwise, it is null.
    /// </returns>
    Task<Portfolio?> GetPortfolioByIdAsync(Guid id);

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
    /// Asynchronously adds a new portfolio to the specified artist.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist to whom the portfolio will be added.</param>
    /// <param name="title">The title of the portfolio.</param>
    /// <param name="description">A description of the portfolio.</param>
    /// <param name="techniqueId">The unique identifier of the technique associated with the portfolio.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddPortfolioAsync(Guid artistId, 
        string title, 
        string description, 
        Guid techniqueId);

    /// <summary>
    /// Asynchronously updates an existing portfolio with new information.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio to update.</param>
    /// <param name="artistId">The unique identifier of the artist associated with the portfolio.</param>
    /// <param name="title">The new title of the portfolio.</param>
    /// <param name="description">The new description of the portfolio.</param>
    /// <param name="techniqueId">The unique identifier of the technique associated with the portfolio.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdatePortfolioAsync(Guid id, 
        Guid artistId, 
        string title, 
        string description, 
        Guid techniqueId);

    /// <summary>
    /// Asynchronously deletes a portfolio by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeletePortfolioAsync(Guid id);
}
