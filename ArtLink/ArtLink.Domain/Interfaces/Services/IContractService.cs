using ArtLink.Domain.Models;
using ArtLink.Domain.Models.Enums;

namespace ArtLink.Domain.Interfaces.Services;

public interface IContractService
{
    /// <summary>
    /// Asynchronously retrieves a contract by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the contract to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the <see cref="Contract"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Contract?> GetContractByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all contracts associated with a specific artist.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist whose contracts are to be retrieved.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of <see cref="Contract"/> objects related to the specified artist.
    /// </returns>
    Task<IEnumerable<Contract>> GetAllByArtistIdAsync(Guid artistId);

    /// <summary>
    /// Asynchronously retrieves all contracts associated with a specific employer.
    /// </summary>
    /// <param name="employerId">The unique identifier of the employer whose contracts are to be retrieved.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains a collection of <see cref="Contract"/> objects related to the specified employer.
    /// </returns>
    Task<IEnumerable<Contract>> GetAllByEmployerIdAsync(Guid employerId);

    /// <summary>
    /// Asynchronously adds a new contract with the specified details.
    /// </summary>
    /// <param name="artistId">The unique identifier of the artist associated with the contract.</param>
    /// <param name="employerId">The unique identifier of the employer associated with the contract.</param>
    /// <param name="projectDescription">A description of the project under the contract.</param>
    /// <param name="status">The initial status of the contract.</param>
    /// <param name="startDate">The start date of the contract (optional).</param>
    /// <param name="endDate">The end date of the contract (optional).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Guid> AddContractAsync(Guid artistId,
        Guid employerId,
        string projectDescription,
        ContractState status,
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// Asynchronously updates an existing contract with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the contract to update.</param>
    /// <param name="artistId">The unique identifier of the artist associated with the contract.</param>
    /// <param name="employerId">The unique identifier of the employer associated with the contract.</param>
    /// <param name="projectDescription">The updated description of the project.</param>
    /// <param name="status">The new status of the contract.</param>
    /// <param name="startDate">The new start date of the contract (optional).</param>
    /// <param name="endDate">The new end date of the contract (optional).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateContractAsync(Guid id,
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
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteContractAsync(Guid id);
}
