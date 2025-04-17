using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArtLink.DataAccess.Repositories;

public class ContractRepository(ArtLinkDbContext context, ILogger<ContractRepository> logger) : IContractRepository
{
    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        try
        {
            var contractDb = await context.Contracts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            return contractDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get contract by id {contractId}.", id);
            throw;
        }
    }

    public async Task<IEnumerable<Contract>> GetAllByArtistIdAsync(Guid artistId)
    {
        try
        {
            var contractsDb = await context.Contracts
                .AsNoTracking()
                .Where(c => c.ArtistId == artistId)
                .ToListAsync();
            return contractsDb.Select(c => c.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get contracts for artist {artistId}.", artistId);
            throw;
        }
    }

    public async Task<IEnumerable<Contract>> GetAllByEmployerIdAsync(Guid employerId)
    {
        try
        {
            var contractsDb = await context.Contracts
                .AsNoTracking()
                .Where(c => c.EmployerId == employerId)
                .ToListAsync();
            return contractsDb.Select(c => c.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get contracts for employer {employerId}.", employerId);
            throw;
        }
    }

    public async Task AddAsync(Guid artistId,
        Guid employerId,
        string projectDescription,
        ContractState status,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        try
        {
            var contract = new ContractDb(
                Guid.NewGuid(),
                artistId,
                employerId,
                projectDescription,
                startDate,
                endDate,
                status.ToDb());

            await context.Contracts.AddAsync(contract);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add contract for artist {artistId} and employer {employerId}.", artistId, employerId);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id,
        Guid artistId,
        Guid employerId,
        string projectDescription,
        ContractState status,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        try
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
            else
            {
                logger.LogWarning("Contract with id {contractId} not found for update.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update contract {contractId}.", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var contract = await context.Contracts.FindAsync(id);
            if (contract != null)
            {
                context.Contracts.Remove(contract);
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogWarning("Contract with id {contractId} not found for deletion.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete contract {contractId}.", id);
            throw;
        }
    }
}
