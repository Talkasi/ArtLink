using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArtLink.DataAccess.Repositories;

public class PortfolioRepository(ArtLinkDbContext context, ILogger<PortfolioRepository> logger) : IPortfolioRepository
{
    public async Task<Portfolio?> GetByIdAsync(Guid id)
    {
        try
        {
            var portfolioDb = await context.Portfolios
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            return portfolioDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get portfolio by id {portfolioId}.", id);
            throw;
        }
    }

    public async Task<IEnumerable<Portfolio>> GetAllByArtistIdAsync(Guid artistId)
    {
        try
        {
            var portfoliosDb = await context.Portfolios
                .AsNoTracking()
                .Where(p => p.ArtistId == artistId)
                .ToListAsync();
            return portfoliosDb.Select(p => p.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get portfolios for artist {artistId}.", artistId);
            throw;
        }
    }

    public async Task AddAsync(Guid artistId,
        string title,
        Guid techniqueId,
        string? description)
    {
        try
        {
            var portfolio = new PortfolioDb(Guid.NewGuid(), artistId, techniqueId, title, description);
            await context.Portfolios.AddAsync(portfolio);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add portfolio for artist {artistId}.", artistId);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id,
        Guid artistId,
        string title,
        Guid techniqueId,
        string? description)
    {
        try
        {
            var portfolioDb = await context.Portfolios.FindAsync(id);
            if (portfolioDb != null)
            {
                portfolioDb.ArtistId = artistId;
                portfolioDb.Title = title;
                portfolioDb.Description = description;
                portfolioDb.TechniqueId = techniqueId;

                context.Portfolios.Update(portfolioDb);
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogWarning("Portfolio with id {portfolioId} not found for update.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update portfolio {portfolioId}.", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var portfolioDb = await context.Portfolios.FindAsync(id);
            if (portfolioDb != null)
            {
                context.Portfolios.Remove(portfolioDb);
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogWarning("Portfolio with id {portfolioId} not found for deletion.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete portfolio {portfolioId}.", id);
            throw;
        }
    }
}
