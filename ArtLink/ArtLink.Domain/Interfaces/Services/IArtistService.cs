using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Services;

public interface IArtistService
{
    /// <summary>
    /// Asynchronously retrieves an artist by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the artist to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the artist if found; otherwise, it is null.
    /// </returns>
    Task<Artist?> GetArtistByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all artists in the system.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of all artists.
    /// </returns>
    Task<IEnumerable<Artist>> GetAllArtistsAsync();

    /// <summary>
    /// Asynchronously adds a new artist to the system.
    /// </summary>
    /// <param name="firstName">The first name of the artist.</param>
    /// <param name="lastName">The last name of the artist.</param>
    /// <param name="email">The email address of the artist.</param>
    /// <param name="bio">A brief biography of the artist.</param>
    /// <param name="profilePicturePath">The file path or URL of the artist's profile picture.</param>
    /// <param name="experience">The years of experience the artist has.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddArtistAsync(string firstName, 
        string lastName, 
        string email, 
        string passwordHash,
        string? bio,
        string? profilePicturePath, 
        int? experience);

    /// <summary>
    /// Asynchronously updates an existing artist's information.
    /// </summary>
    /// <param name="id">The unique identifier of the artist to update.</param>
    /// <param name="firstName">The new first name of the artist.</param>
    /// <param name="lastName">The new last name of the artist.</param>
    /// <param name="email">The new email address of the artist.</param>
    /// <param name="bio">The new biography of the artist.</param>
    /// <param name="profilePicturePath">The new file path or URL of the artist's profile picture.</param>
    /// <param name="experience">The new years of experience the artist has.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateArtistAsync(Guid id, 
        string firstName, 
        string lastName, 
        string email, 
        string? bio, 
        string? profilePicturePath, 
        int? experience);

    /// <summary>
    /// Asynchronously deletes an artist by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the artist to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteArtistAsync(Guid id);
}
