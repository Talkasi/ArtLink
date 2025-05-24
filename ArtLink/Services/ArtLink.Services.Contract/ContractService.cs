using ContractModel = ArtLink.Domain.Models.Contract;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models.Enums;
using Microsoft.Extensions.Logging;

namespace ArtLink.Services.Contract;

public class ContractService(IContractRepository contractRepository, ILogger<ContractService> logger) : IContractService
{
    public async Task<ContractModel?> GetContractByIdAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[ContractService][GetById] Attempting to get contract with ID: {Id}", id);
            var contract = await contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                logger.LogWarning("[ContractService][GetById] Contract not found with ID: {Id}", id);
            }
            return contract;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ContractService][GetById] Error getting contract with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ContractModel>> GetAllByArtistIdAsync(Guid artistId)
    {
        try
        {
            logger.LogInformation("[ContractService][GetAllByArtistId] Attempting to get all contracts for artist with ID: {ArtistId}", artistId);
            var contracts = (await contractRepository.GetAllByArtistIdAsync(artistId)).ToList();
            logger.LogInformation("[ContractService][GetAllByArtistId] Found {Count} contracts for artist with ID: {ArtistId}", contracts.Count, artistId);
            return contracts;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ContractService][GetAllByArtistId] Error fetching contracts for artist with ID: {ArtistId}", artistId);
            throw;
        }
    }

    public async Task<IEnumerable<ContractModel>> GetAllByEmployerIdAsync(Guid employerId)
    {
        try
        {
            logger.LogInformation("[ContractService][GetAllByEmployerId] Attempting to get all contracts for employer with ID: {EmployerId}", employerId);
            var contracts = (await contractRepository.GetAllByEmployerIdAsync(employerId)).ToList();
            logger.LogInformation("[ContractService][GetAllByEmployerId] Found {Count} contracts for employer with ID: {EmployerId}", contracts.Count, employerId);
            return contracts;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ContractService][GetAllByEmployerId] Error fetching contracts for employer with ID: {EmployerId}", employerId);
            throw;
        }
    }

    public async Task<Guid> AddContractAsync(Guid artistId,
        Guid employerId,
        string projectDescription,
        ContractState status,
        DateTime? startDate,
        DateTime? endDate = null)
    {
        try
        {
            logger.LogInformation("[ContractService][Add] Adding new contract for artist with ID: {ArtistId}, employer with ID: {EmployerId}", artistId, employerId);
            var id = await contractRepository.AddAsync(artistId, employerId, projectDescription, status, startDate, endDate);
            logger.LogInformation("[ContractService][Add] Successfully added contract for artist with ID: {ArtistId}, employer with ID: {EmployerId}", artistId, employerId);
            return id;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ContractService][Add] Error adding contract for artist with ID: {ArtistId}, employer with ID: {EmployerId}", artistId, employerId);
            throw;
        }
    }

    public async Task UpdateContractAsync(Guid id,
        Guid artistId,
        Guid employerId,
        string projectDescription,
        ContractState status,
        DateTime? startDate,
        DateTime? endDate = null)
    {
        try
        {
            logger.LogInformation("[ContractService][Update] Updating contract with ID: {Id}", id);
            await contractRepository.UpdateAsync(id, artistId, employerId, projectDescription, status, startDate, endDate);
            logger.LogInformation("[ContractService][Update] Successfully updated contract with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ContractService][Update] Error updating contract with ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteContractAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[ContractService][Delete] Deleting contract with ID: {Id}", id);
            await contractRepository.DeleteAsync(id);
            logger.LogInformation("[ContractService][Delete] Successfully deleted contract with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ContractService][Delete] Error deleting contract with ID: {Id}", id);
            throw;
        }
    }
}
