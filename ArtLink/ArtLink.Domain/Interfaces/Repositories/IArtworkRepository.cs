using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Repositories;

public interface IArtworkRepository
{
    /// <summary>
    /// Asynchronously retrieves an artwork by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the artwork to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the artwork if found; otherwise, it returns null.
    /// </returns>
    Task<Artwork?> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all artworks associated with a specific portfolio.
    /// </summary>
    /// <param name="portfolioId">The unique identifier of the portfolio whose artworks are to be retrieved.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of artworks associated with the specified portfolio.
    /// </returns>
    Task<IEnumerable<Artwork>> GetAllByPortfolioIdAsync(Guid portfolioId);

    /// <summary>
    /// Asynchronously adds a new artwork to the specified portfolio.
    /// </summary>
    /// <param name="portfolioId">The unique identifier of the portfolio to which the artwork will be added.</param>
    /// <param name="title">The title of the artwork.</param>
    /// <param name="imagePath">The file path or URL of the artwork's image.</param>
    /// <param name="description">A description of the artwork.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Guid> AddAsync(Guid portfolioId,
        string title,
        string imagePath,
        string? description = null);

    /// <summary>
    /// Asynchronously updates an existing artwork.
    /// </summary>
    /// <param name="id">The unique identifier of the artwork to update.</param>
    /// <param name="portfolioId">The unique identifier of the portfolio to which the artwork belongs.</param>
    /// <param name="title">The updated title of the artwork.</param>
    /// <param name="imagePath">The updated file path or URL of the artwork's image.</param>
    /// <param name="description">The updated description of the artwork.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Guid id,
        Guid portfolioId,
        string title,
        string imagePath,
        string? description = null);

    /// <summary>
    /// Asynchronously deletes an artwork by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the artwork to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Asynchronously searches for artworks based on a search prompt.
    /// </summary>
    /// <param name="prompt">The search prompt to filter artworks.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of artworks matching the search criteria.</returns>
    Task<IEnumerable<Artwork>> SearchByPromptAsync(string prompt);
}
