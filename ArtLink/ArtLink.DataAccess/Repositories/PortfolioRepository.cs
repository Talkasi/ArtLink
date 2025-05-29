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
    private const string ClassName = nameof(PortfolioRepository);

    public async Task<Portfolio?> GetByIdAsync(Guid id)
    {
        const string method = nameof(GetByIdAsync);
        try
        {
            var portfolioDb = await context.Portfolios
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (portfolioDb != null)
                logger.LogInformation("[{Class}][{Method}] Retrieved portfolio with ID {PortfolioId}.", ClassName, method, id);
            else
                logger.LogWarning("[{Class}][{Method}] Portfolio with ID {PortfolioId} not found.", ClassName, method, id);

            return portfolioDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get portfolio by ID {PortfolioId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task<IEnumerable<Portfolio>> GetAllByArtistIdAsync(Guid artistId)
    {
        const string method = nameof(GetAllByArtistIdAsync);
        try
        {
            var portfoliosDb = await context.Portfolios
                .AsNoTracking()
                .Where(p => p.ArtistId == artistId)
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Retrieved {Count} portfolios for artist ID {ArtistId}.", ClassName, method, portfoliosDb.Count, artistId);

            return portfoliosDb.Select(p => p.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get portfolios for artist {ArtistId}.", ClassName, method, artistId);
            throw;
        }
    }

    public async Task<Guid> AddAsync(Guid artistId,
        string title,
        Guid techniqueId,
        string? description)
    {
        const string method = nameof(AddAsync);
        try
        {
            var portfolio = new PortfolioDb(Guid.NewGuid(), artistId, techniqueId, title, description);
            await context.Portfolios.AddAsync(portfolio);
            await context.SaveChangesAsync();

            logger.LogInformation("[{Class}][{Method}] Added portfolio '{Title}' for artist ID {ArtistId}.", ClassName, method, title, artistId);

            return portfolio.Id;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to add portfolio for artist {ArtistId}.", ClassName, method, artistId);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id,
        Guid artistId,
        string title,
        Guid techniqueId,
        string? description)
    {
        const string method = nameof(UpdateAsync);
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

                logger.LogInformation("[{Class}][{Method}] Updated portfolio ID {PortfolioId}.", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Portfolio with ID {PortfolioId} not found for update.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to update portfolio {PortfolioId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        const string method = nameof(DeleteAsync);
        try
        {
            var portfolioDb = await context.Portfolios.FindAsync(id);
            if (portfolioDb != null)
            {
                context.Portfolios.Remove(portfolioDb);
                await context.SaveChangesAsync();

                logger.LogInformation("[{Class}][{Method}] Deleted portfolio with ID {PortfolioId}.", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Portfolio with ID {PortfolioId} not found for deletion.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to delete portfolio {PortfolioId}.", ClassName, method, id);
            throw;
        }
    }
}
