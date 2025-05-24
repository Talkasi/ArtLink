using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Repositories;

public interface IEmployerRepository
{
    /// <summary>
    /// Asynchronously retrieves an employer by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employer to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the <see cref="Employer"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Employer?> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all employers in the system.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of <see cref="Employer"/> entities.
    /// </returns>
    Task<IEnumerable<Employer>> GetAllEmployersAsync();

    /// <summary>
    /// Asynchronously adds a new employer to the system.
    /// </summary>
    /// <param name="companyName">The name of the company associated with the employer.</param>
    /// <param name="email">The email address of the employer.</param>
    /// <param name="passwordHash">The hashed password of the employer.</param>
    /// <param name="cpFirstName">The first name of the contact person.</param>
    /// <param name="cpLastName">The last name of the contact person.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Guid> AddAsync(
        string companyName,
        string email,
        string passwordHash,
        string cpFirstName,
        string cpLastName);

    /// <summary>
    /// Asynchronously updates an existing employer's information.
    /// </summary>
    /// <param name="id">The unique identifier of the employer to update.</param>
    /// <param name="companyName">The updated name of the company.</param>
    /// <param name="email">The updated email address of the employer.</param>
    /// <param name="cpFirstName">The updated contact person's first name.</param>
    /// <param name="cpLastName">The updated contact person's last name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(
        Guid id,
        string companyName,
        string email,
        string cpFirstName,
        string cpLastName);

    /// <summary>
    /// Asynchronously deletes an employer by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employer to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Asynchronously searches for employers based on a search prompt.
    /// </summary>
    /// <param name="prompt">The search prompt to filter employers.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of <see cref="Employer"/> entities matching the search criteria.
    /// </returns>
    Task<IEnumerable<Employer>> SearchByPromptAsync(string prompt);

    /// <summary>
    /// Asynchronously retrieves an employer by their email and password hash.
    /// Used for authentication purposes.
    /// </summary>
    /// <param name="email">The email of the employer.</param>
    /// <param name="passwordHash">The hashed password of the employer.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the <see cref="Employer"/> if credentials are valid; otherwise, <c>null</c>.
    /// </returns>
    Task<Employer?> LoginAsync(string email, string passwordHash);
}
