using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Services;

public interface IArtworkService
{
    /// <summary>
    /// Asynchronously retrieves an artwork by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the artwork to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the artwork if found; otherwise, it is null.
    /// </returns>
    Task<Artwork?> GetArtworkByIdAsync(Guid id);

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
    /// <param name="description">A description of the artwork.</param>
    /// <param name="imagePath">The file path or URL of the artwork's image.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddArtworkAsync(Guid portfolioId, 
        string title, 
        string description, 
        string imagePath);

    /// <summary>
    /// Asynchronously updates an existing artwork.
    /// </summary>
    /// <param name="id">The unique identifier of the artwork to update.</param>
    /// <param name="portfolioId">The unique identifier of the portfolio to which the artwork belongs.</param>
    /// <param name="title">The new title of the artwork.</param>
    /// <param name="description">The new description of the artwork.</param>
    /// <param name="imagePath">The new file path or URL of the artwork's image.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateArtworkAsync(Guid id, 
        Guid portfolioId, 
        string title, 
        string description, 
        string imagePath);

    /// <summary>
    /// Asynchronously deletes an artwork by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the artwork to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteArtworkAsync(Guid id);
}

