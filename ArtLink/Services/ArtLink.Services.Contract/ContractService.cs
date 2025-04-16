using ContractModel = ArtLink.Domain.Models.Contract;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models.Enums;

namespace ArtLink.Services.Contract;

public class ContractService(IContractRepository contractRepository) : IContractService
{
    public async Task<ContractModel?> GetContractByIdAsync(Guid id)
    {
        return await contractRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<ContractModel>> GetAllByArtistIdAsync(Guid artistId)
    {
        return await contractRepository.GetAllByArtistIdAsync(artistId);
    }

    public async Task<IEnumerable<ContractModel>> GetAllByEmployerIdAsync(Guid employerId)
    {
        return await contractRepository.GetAllByEmployerIdAsync(employerId);
    }

    public async Task AddContractAsync(Guid artistId,
        Guid employerId,
        string projectDescription,
        ContractState status,
        DateTime? startDate,
        DateTime? endDate = null)
    {
        await contractRepository.AddAsync(artistId, employerId, projectDescription, status: status, startDate: startDate, endDate: endDate);
    }

    public async Task UpdateContractAsync(Guid id,
        Guid artistId,
        Guid employerId,
        string projectDescription,
        ContractState status,
        DateTime? startDate,
        DateTime? endDate = null)
    {
        await contractRepository.UpdateAsync(id, artistId, employerId, projectDescription, status: status, startDate: startDate, endDate: endDate);
    }

    public async Task DeleteContractAsync(Guid id)
    {
        await contractRepository.DeleteAsync(id);
    }
}
