using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using ArtworkModel = ArtLink.Domain.Models.Artwork;

namespace ArtLink.Services.Artwork;

public class ArtworkService(IArtworkRepository artworkRepository, ILogger<ArtworkService> logger) : IArtworkService
{
    public async Task<ArtworkModel?> GetArtworkByIdAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[ArtworkService][GetById] Attempting to get artwork with ID: {Id}", id);
            var artwork = await artworkRepository.GetByIdAsync(id);
            if (artwork == null)
            {
                logger.LogWarning("[ArtworkService][GetById] Artwork not found with ID: {Id}", id);
            }
            return artwork;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkService][GetById] Error getting artwork with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ArtworkModel>> GetAllByPortfolioIdAsync(Guid portfolioId)
    {
        try
        {
            logger.LogInformation("[ArtworkService][GetAllByPortfolioId] Attempting to get all artworks for portfolio with ID: {PortfolioId}", portfolioId);
            var artworks = (await artworkRepository.GetAllByPortfolioIdAsync(portfolioId)).ToList();
            logger.LogInformation("[ArtworkService][GetAllByPortfolioId] Found {Count} artworks for portfolio with ID: {PortfolioId}", artworks.Count, portfolioId);
            return artworks;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkService][GetAllByPortfolioId] Error fetching artworks for portfolio with ID: {PortfolioId}", portfolioId);
            throw;
        }
    }

    public async Task<Guid> AddArtworkAsync(Guid portfolioId, string title, string imagePath, string? description)
    {
        try
        {
            logger.LogInformation("[ArtworkService][Add] Adding new artwork to portfolio with ID: {PortfolioId}, Title: {Title}", portfolioId, title);
            var id = await artworkRepository.AddAsync(portfolioId, title, imagePath, description);
            logger.LogInformation("[ArtworkService][Add] Successfully added artwork to portfolio with ID: {PortfolioId}, Title: {Title}", portfolioId, title);
            return id;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkService][Add] Error adding artwork to portfolio with ID: {PortfolioId}, Title: {Title}", portfolioId, title);
            throw;
        }
    }

    public async Task UpdateArtworkAsync(Guid id, Guid portfolioId, string title, string imagePath, string? description)
    {
        try
        {
            logger.LogInformation("[ArtworkService][Update] Updating artwork with ID: {Id}", id);
            await artworkRepository.UpdateAsync(id, portfolioId, title, imagePath, description);
            logger.LogInformation("[ArtworkService][Update] Successfully updated artwork with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkService][Update] Error updating artwork with ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteArtworkAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[ArtworkService][Delete] Deleting artwork with ID: {Id}", id);
            await artworkRepository.DeleteAsync(id);
            logger.LogInformation("[ArtworkService][Delete] Successfully deleted artwork with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkService][Delete] Error deleting artwork with ID: {Id}", id);
            throw;
        }
    }
}
