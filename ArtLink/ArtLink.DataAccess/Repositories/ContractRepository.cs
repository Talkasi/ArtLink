using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.DataAccess.Models.Enums;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArtLink.DataAccess.Repositories;

public class ContractRepository(ArtLinkDbContext context) : IContractRepository
{
    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        var contractDb = await context.Contracts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        return contractDb?.ToDomain();
    }

    public async Task<IEnumerable<Contract>> GetAllByArtistIdAsync(Guid artistId)
    {
        var contractsDb = await context.Contracts
            .AsNoTracking()
            .Where(c => c.ArtistId == artistId)
            .ToListAsync();
        return contractsDb.Select(c => c.ToDomain());
    }

    public async Task<IEnumerable<Contract>> GetAllByEmployerIdAsync(Guid employerId)
    {
        var contractsDb = await context.Contracts
            .AsNoTracking()
            .Where(c => c.EmployerId == employerId)
            .ToListAsync();
        return contractsDb.Select(c => c.ToDomain());
    }

    public async Task AddAsync(Guid artistId, 
        Guid employerId, 
        string projectDescription, 
        DateTime? startDate, 
        DateTime? endDate, 
        ContractState status)
    {
        var contract = new ContractDb(Guid.NewGuid(), artistId, employerId, projectDescription, startDate, endDate, status.ToDb());
        await context.Contracts.AddAsync(contract);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, 
        Guid artistId, 
        Guid employerId, 
        string projectDescription, 
        DateTime? startDate, 
        DateTime? endDate, 
        ContractState status)
    {
        var contract = await context.Contracts.FindAsync(id);
        if (contract != null)
        {
            contract.ArtistId = artistId;
            contract.EmployerId = employerId;
            contract.ProjectDescription = projectDescription;
            contract.StartDate = startDate;
            contract.EndDate = endDate;
            contract.Status = status.ToDb();

            context.Contracts.Update(contract);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var contract = await context.Contracts.FindAsync(id);
        if (contract != null)
        {
            context.Contracts.Remove(contract);
            await context.SaveChangesAsync();
        }
    }
}

