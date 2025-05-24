using ArtLink.Domain.Models;

namespace ArtLink.Domain.Interfaces.Services;

public interface IEmployerService
{
    /// <summary>
    /// Asynchronously retrieves an employer by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employer to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <see cref="Employer"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Employer?> GetEmployerByIdAsync(Guid id);

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
    Task<Guid> AddEmployerAsync(
        string companyName,
        string email,
        string passwordHash,
        string cpFirstName,
        string cpLastName);

    /// <summary>
    /// Asynchronously updates an existing employer's information.
    /// </summary>
    /// <param name="id">The unique identifier of the employer to update.</param>
    /// <param name="companyName">The updated company name.</param>
    /// <param name="email">The updated email address.</param>
    /// <param name="cpFirstName">The updated contact person's first name.</param>
    /// <param name="cpLastName">The updated contact person's last name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateEmployerAsync(
        Guid id,
        string companyName,
        string email,
        string cpFirstName,
        string cpLastName);

    /// <summary>
    /// Asynchronously deletes an employer from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the employer to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteEmployerAsync(Guid id);

    /// <summary>
    /// Asynchronously logs in an employer using their email and password hash.
    /// </summary>
    /// <param name="email">The email address of the employer.</param>
    /// <param name="passwordHash">The hashed password of the employer.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <see cref="Employer"/> if credentials are valid; otherwise, <c>null</c>.
    /// </returns>
    Task<Employer?> LoginEmployerAsync(string email, string passwordHash);
}
