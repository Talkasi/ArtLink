using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArtLink.DataAccess.Repositories;

public class ArtworkRepository(ArtLinkDbContext context, ILogger<ArtworkRepository> logger) : IArtworkRepository
{
    private const string ClassName = nameof(ArtworkRepository);

    public async Task<Artwork?> GetByIdAsync(Guid id)
    {
        const string method = nameof(GetByIdAsync);
        try
        {
            var artworkDb = await context.Artworks.FirstOrDefaultAsync(a => a.Id == id);

            if (artworkDb != null)
                logger.LogInformation("[{Class}][{Method}] Retrieved artwork with ID {ArtworkId}.", ClassName, method, id);
            else
                logger.LogWarning("[{Class}][{Method}] Artwork with ID {ArtworkId} not found.", ClassName, method, id);

            return artworkDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get artwork by ID {ArtworkId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task<IEnumerable<Artwork>> GetAllByPortfolioIdAsync(Guid portfolioId)
    {
        const string method = nameof(GetAllByPortfolioIdAsync);
        try
        {
            var artworksDb = await context.Artworks
                .Where(a => a.PortfolioId == portfolioId)
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Retrieved {Count} artworks for portfolio ID {PortfolioId}.", ClassName, method, artworksDb.Count, portfolioId);

            return artworksDb.Select(a => a.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get artworks by portfolio ID {PortfolioId}.", ClassName, method, portfolioId);
            throw;
        }
    }

    public async Task AddAsync(Guid portfolioId,
        string title,
        string imagePath,
        string? description)
    {
        const string method = nameof(AddAsync);
        try
        {
            var artworkDb = new ArtworkDb(Guid.NewGuid(), portfolioId, title, imagePath, description);
            await context.Artworks.AddAsync(artworkDb);
            await context.SaveChangesAsync();

            logger.LogInformation("[{Class}][{Method}] Added new artwork '{Title}' to portfolio {PortfolioId}.", ClassName, method, title, portfolioId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to add artwork to portfolio {PortfolioId}.", ClassName, method, portfolioId);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id,
        Guid portfolioId,
        string title,
        string imagePath,
        string? description)
    {
        const string method = nameof(UpdateAsync);
        try
        {
            var artwork = await context.Artworks.FindAsync(id);
            if (artwork != null)
            {
                artwork.PortfolioId = portfolioId;
                artwork.Title = title;
                artwork.Description = description;
                artwork.ImagePath = imagePath;

                await context.SaveChangesAsync();

                logger.LogInformation("[{Class}][{Method}] Updated artwork with ID {ArtworkId}.", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Artwork with ID {ArtworkId} not found for update.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to update artwork {ArtworkId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        const string method = nameof(DeleteAsync);
        try
        {
            var artwork = await context.Artworks.FindAsync(id);
            if (artwork != null)
            {
                context.Artworks.Remove(artwork);
                await context.SaveChangesAsync();

                logger.LogInformation("[{Class}][{Method}] Deleted artwork with ID {ArtworkId}.", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Artwork with ID {ArtworkId} not found for deletion.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to delete artwork {ArtworkId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task<IEnumerable<Artwork>> SearchByPromptAsync(string prompt)
    {
        const string method = nameof(SearchByPromptAsync);
        try
        {
            var artworksDb = await context.Artworks
                .Where(a => a.Title.Contains(prompt) ||
                            (a.Description != null && a.Description.Contains(prompt)))
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Searched artworks with prompt '{Prompt}', found: {Count}.", ClassName, method, prompt, artworksDb.Count);

            return artworksDb.Select(a => a.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to search artworks with prompt '{Prompt}'.", ClassName, method, prompt);
            throw;
        }
    }
}
