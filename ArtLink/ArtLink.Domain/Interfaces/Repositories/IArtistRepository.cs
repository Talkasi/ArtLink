using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Repositories;

public interface IArtistRepository
{
    /// <summary>
    /// Asynchronously retrieves an artist by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the artist to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the artist if found; otherwise, it returns null.
    /// </returns>
    Task<Artist?> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all artists in the repository.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of all artists.
    /// </returns>
    Task<IEnumerable<Artist>> GetAllAsync();

    /// <summary>
    /// Asynchronously adds a new artist to the repository.
    /// </summary>
    /// <param name="firstName">The first name of the artist.</param>
    /// <param name="lastName">The last name of the artist.</param>
    /// <param name="email">The email address of the artist.</param>
    /// <param name="bio">A brief biography of the artist.</param>
    /// <param name="profilePicturePath">The file path or URL of the artist's profile picture.</param>
    /// <param name="experience">The number of years of experience the artist has.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(string firstName, 
        string lastName, 
        string email, 
        string bio, 
        string profilePicturePath, 
        int experience);

    /// <summary>
    /// Asynchronously updates an existing artist in the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the artist to update.</param>
    /// <param name="firstName">The updated first name of the artist.</param>
    /// <param name="lastName">The updated last name of the artist.</param>
    /// <param name="email">The updated email address of the artist.</param>
    /// <param name="bio">The updated biography of the artist.</param>
    /// <param name="profilePicturePath">The updated file path or URL of the artist's profile picture.</param>
    /// <param name="experience">The updated number of years of experience the artist has.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Guid id, 
        string firstName, 
        string lastName, 
        string email, 
        string bio, 
        string profilePicturePath, 
        int experience);

    /// <summary>
    /// Asynchronously deletes an artist by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the artist to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Asynchronously searches for artists based on a search prompt.
    /// </summary>
    /// <param name="prompt">The search prompt to filter artists.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of artists matching the search criteria.</returns>
    Task<IEnumerable<Artist>> SearchByPromptAsync(string prompt);
}
