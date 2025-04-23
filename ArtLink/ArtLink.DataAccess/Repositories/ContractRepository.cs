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
    private const string ClassName = nameof(ContractRepository);

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        const string method = nameof(GetByIdAsync);
        try
        {
            var contractDb = await context.Contracts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contractDb != null)
                logger.LogInformation("[{Class}][{Method}] Retrieved contract with ID {ContractId}.", ClassName, method, id);
            else
                logger.LogWarning("[{Class}][{Method}] Contract with ID {ContractId} not found.", ClassName, method, id);

            return contractDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get contract by ID {ContractId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task<IEnumerable<Contract>> GetAllByArtistIdAsync(Guid artistId)
    {
        const string method = nameof(GetAllByArtistIdAsync);
        try
        {
            var contractsDb = await context.Contracts
                .AsNoTracking()
                .Where(c => c.ArtistId == artistId)
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Retrieved {Count} contracts for artist ID {ArtistId}.", ClassName, method, contractsDb.Count, artistId);

            return contractsDb.Select(c => c.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get contracts for artist {ArtistId}.", ClassName, method, artistId);
            throw;
        }
    }

    public async Task<IEnumerable<Contract>> GetAllByEmployerIdAsync(Guid employerId)
    {
        const string method = nameof(GetAllByEmployerIdAsync);
        try
        {
            var contractsDb = await context.Contracts
                .AsNoTracking()
                .Where(c => c.EmployerId == employerId)
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Retrieved {Count} contracts for employer ID {EmployerId}.", ClassName, method, contractsDb.Count, employerId);

            return contractsDb.Select(c => c.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get contracts for employer {EmployerId}.", ClassName, method, employerId);
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
        const string method = nameof(AddAsync);
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

            logger.LogInformation("[{Class}][{Method}] Added contract between artist {ArtistId} and employer {EmployerId} with status {Status}.",
                ClassName, method, artistId, employerId, status);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to add contract for artist {ArtistId} and employer {EmployerId}.",
                ClassName, method, artistId, employerId);
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
        const string method = nameof(UpdateAsync);
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

                logger.LogInformation("[{Class}][{Method}] Updated contract with ID {ContractId}.", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Contract with ID {ContractId} not found for update.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to update contract {ContractId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        const string method = nameof(DeleteAsync);
        try
        {
            var contract = await context.Contracts.FindAsync(id);
            if (contract != null)
            {
                context.Contracts.Remove(contract);
                await context.SaveChangesAsync();

                logger.LogInformation("[{Class}][{Method}] Deleted contract with ID {ContractId}.", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Contract with ID {ContractId} not found for deletion.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to delete contract {ContractId}.", ClassName, method, id);
            throw;
        }
    }
}
