using ArtLink.Domain.Models;
using ArtLink.Domain.Models.Enums;

namespace ArtLink.Domain.Interfaces.Repositories;

public interface IContractRepository
{
    /// <summary>
    /// Asynchronously retrieves a contract by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the contract to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the contract if found; otherwise, it returns null.
    /// </returns>
    Task<Contract?> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all contracts associated with a specific artist.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist whose contracts are to be retrieved.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of contracts associated with the specified artist.
    /// </returns>
    Task<IEnumerable<Contract>> GetAllByArtistIdAsync(Guid artistId);

    /// <summary>
    /// Asynchronously retrieves all contracts associated with a specific employer.
    /// </summary>
    /// <param name="employerId">The unique identifier of the employer whose contracts are to be retrieved.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of contracts associated with the specified employer.
    /// </returns>
    Task<IEnumerable<Contract>> GetAllByEmployerIdAsync(Guid employerId);

    /// <summary>
    /// Asynchronously adds a new contract to the repository.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist associated with the contract.</param>
    /// <param name="employerId">The unique identifier of the employer associated with the contract.</param>
    /// <param name="projectDescription">A description of the project covered by the contract.</param>
    /// <param name="status">The current status of the contract.</param>
    /// <param name="startDate">The start date of the contract.</param>
    /// <param name="endDate">The end date of the contract.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Guid artistId,
        Guid employerId,
        string projectDescription,
        ContractState status,
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// Asynchronously updates an existing contract in the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the contract to update.</param>
    /// <param name="artistId">The unique identifier of the artist associated with the contract.</param>
    /// <param name="employerId">The unique identifier of the employer associated with the contract.</param>
    /// <param name="projectDescription">The updated description of the project covered by the contract.</param>
    /// <param name="status">The updated status of the contract.</param>
    /// <param name="startDate">The updated start date of the contract.</param>
    /// <param name="endDate">The updated end date of the contract.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Guid id,
        Guid artistId,
        Guid employerId,
        string projectDescription,
        ContractState status,
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// Asynchronously deletes a contract by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the contract to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}
