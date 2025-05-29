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
    /// The task result contains the <see cref="Artist"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Artist?> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all artists in the repository.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a collection of <see cref="Artist"/> entities.
    /// </returns>
    Task<IEnumerable<Artist>> GetAllAsync();

    /// <summary>
    /// Asynchronously adds a new artist to the repository.
    /// </summary>
    /// <param name="firstName">The first name of the artist.</param>
    /// <param name="lastName">The last name of the artist.</param>
    /// <param name="email">The email address of the artist.</param>
    /// <param name="passwordHash">The hashed password of the artist.</param>
    /// <param name="bio">A brief biography of the artist (optional).</param>
    /// <param name="profilePicturePath">The file path or URL of the artist's profile picture (optional).</param>
    /// <param name="experience">The number of years of experience the artist has (optional).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Guid> AddAsync(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        string? bio = null,
        string? profilePicturePath = null,
        int? experience = null);

    /// <summary>
    /// Asynchronously updates an existing artist in the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the artist to update.</param>
    /// <param name="firstName">The updated first name of the artist.</param>
    /// <param name="lastName">The updated last name of the artist.</param>
    /// <param name="email">The updated email address of the artist.</param>
    /// <param name="bio">The updated biography of the artist (optional).</param>
    /// <param name="profilePicturePath">The updated file path or URL of the artist's profile picture (optional).</param>
    /// <param name="experience">The updated number of years of experience the artist has (optional).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string? bio = null,
        string? profilePicturePath = null,
        int? experience = null);

    /// <summary>
    /// Asynchronously deletes an artist by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the artist to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Asynchronously searches for artists based on a prompt.
    /// </summary>
    /// <param name="prompt">The search prompt to filter artists.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a collection of <see cref="Artist"/> entities matching the criteria.
    /// </returns>
    Task<IEnumerable<Artist>> SearchByPromptAsync(string prompt);

    /// <summary>
    /// Asynchronously retrieves an artist by email and password hash.
    /// Used for authentication purposes.
    /// </summary>
    /// <param name="email">The email of the artist.</param>
    /// <param name="passwordHash">The hashed password of the artist.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <see cref="Artist"/> if credentials match; otherwise, <c>null</c>.
    /// </returns>
    Task<Artist?> LoginAsync(string email, string passwordHash);
}
