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
    public async Task<Artwork?> GetByIdAsync(Guid id)
    {
        try
        {
            var artworkDb = await context.Artworks.FirstOrDefaultAsync(a => a.Id == id);
            return artworkDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get artwork by id {artworkId}.", id);
            throw;
        }
    }

    public async Task<IEnumerable<Artwork>> GetAllByPortfolioIdAsync(Guid portfolioId)
    {
        try
        {
            var artworksDb = await context.Artworks
                .Where(a => a.PortfolioId == portfolioId)
                .ToListAsync();
            return artworksDb.Select(a => a.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get artworks by portfolio id {portfolioId}.", portfolioId);
            throw;
        }
    }

    public async Task AddAsync(Guid portfolioId,
        string title,
        string imagePath,
        string? description)
    {
        try
        {
            var artworkDb = new ArtworkDb(Guid.NewGuid(), portfolioId, title, imagePath, description);
            await context.Artworks.AddAsync(artworkDb);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add artwork to portfolio {portfolioId}.", portfolioId);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id,
        Guid portfolioId,
        string title,
        string imagePath,
        string? description)
    {
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
            }
            else
            {
                logger.LogWarning("Artwork with id {artworkId} not found for update.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update artwork {artworkId}.", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var artwork = await context.Artworks.FindAsync(id);
            if (artwork != null)
            {
                context.Artworks.Remove(artwork);
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogWarning("Artwork with id {artworkId} not found for deletion.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete artwork {artworkId}.", id);
            throw;
        }
    }

    public async Task<IEnumerable<Artwork>> SearchByPromptAsync(string prompt)
    {
        try
        {
            var artworksDb = await context.Artworks
                .Where(a => a.Title.Contains(prompt) ||
                            (a.Description != null && a.Description.Contains(prompt)))
                .ToListAsync();
            return artworksDb.Select(a => a.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to search artworks with prompt '{prompt}'.", prompt);
            throw;
        }
    }
}
