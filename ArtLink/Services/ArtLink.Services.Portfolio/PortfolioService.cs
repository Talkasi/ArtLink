using PortfolioModel = ArtLink.Domain.Models.Portfolio;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ArtLink.Services.Portfolio;

public class PortfolioService(IPortfolioRepository portfolioRepository, ILogger<PortfolioService> logger) : IPortfolioService
{
    public async Task<PortfolioModel?> GetPortfolioByIdAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[PortfolioService][GetById] Attempting to get portfolio with ID: {Id}", id);
            var portfolio = await portfolioRepository.GetByIdAsync(id);
            if (portfolio == null)
            {
                logger.LogWarning("[PortfolioService][GetById] Portfolio not found with ID: {Id}", id);
            }
            return portfolio;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioService][GetById] Error getting portfolio with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PortfolioModel>> GetAllByArtistIdAsync(Guid artistId)
    {
        try
        {
            logger.LogInformation("[PortfolioService][GetAllByArtistId] Attempting to get all portfolios for artist ID: {ArtistId}", artistId);
            var portfolios = (await portfolioRepository.GetAllByArtistIdAsync(artistId)).ToList();
            logger.LogInformation("[PortfolioService][GetAllByArtistId] Found {Count} portfolios for artist ID: {ArtistId}", portfolios.Count, artistId);
            return portfolios;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioService][GetAllByArtistId] Error fetching portfolios for artist ID: {ArtistId}", artistId);
            throw;
        }
    }

    public async Task AddPortfolioAsync(Guid artistId, string title, Guid techniqueId, string? description)
    {
        try
        {
            logger.LogInformation("[PortfolioService][Add] Adding new portfolio for artist ID: {ArtistId}, Title: {Title}", artistId, title);
            await portfolioRepository.AddAsync(artistId, title, techniqueId, description);
            logger.LogInformation("[PortfolioService][Add] Successfully added new portfolio for artist ID: {ArtistId}, Title: {Title}", artistId, title);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioService][Add] Error adding portfolio for artist ID: {ArtistId}, Title: {Title}", artistId, title);
            throw;
        }
    }

    public async Task UpdatePortfolioAsync(Guid id, Guid artistId, string title, Guid techniqueId, string? description)
    {
        try
        {
            logger.LogInformation("[PortfolioService][Update] Updating portfolio with ID: {Id}", id);
            await portfolioRepository.UpdateAsync(id, artistId, title, techniqueId, description);
            logger.LogInformation("[PortfolioService][Update] Successfully updated portfolio with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioService][Update] Error updating portfolio with ID: {Id}", id);
            throw;
        }
    }

    public async Task DeletePortfolioAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[PortfolioService][Delete] Deleting portfolio with ID: {Id}", id);
            await portfolioRepository.DeleteAsync(id);
            logger.LogInformation("[PortfolioService][Delete] Successfully deleted portfolio with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioService][Delete] Error deleting portfolio with ID: {Id}", id);
            throw;
        }
    }
}
