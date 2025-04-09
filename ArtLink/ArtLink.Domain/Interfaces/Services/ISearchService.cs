using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Services;

public interface ISearchService
{
    /// <summary>
    /// Asynchronously searches for artists based on a search prompt.
    /// </summary>
    /// <param name="prompt">The search prompt used to find matching artists.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of artists that match the search criteria.
    /// </returns>
    Task<IEnumerable<Artist>> SearchArtistsByPromptAsync(string prompt);

    /// <summary>
    /// Asynchronously searches for employers based on a search prompt.
    /// </summary>
    /// <param name="prompt">The search prompt used to find matching employers.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of employers that match the search criteria.
    /// </returns>
    Task<IEnumerable<Employer>> SearchEmployersByPromptAsync(string prompt);

    /// <summary>
    /// Asynchronously searches for artworks based on a search prompt.
    /// </summary>
    /// <param name="prompt">The search prompt used to find matching artworks.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of artworks that match the search criteria.
    /// </returns>
    Task<IEnumerable<Artwork>> SearchArtWorksByPromptAsync(string prompt);
}
