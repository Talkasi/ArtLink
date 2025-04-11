using PortfolioModel = ArtLink.Domain.Models.Portfolio;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Interfaces.Repositories;

namespace ArtLink.Services.Portfolio;

public class PortfolioService(IPortfolioRepository portfolioRepository) : IPortfolioService
{
    public async Task<PortfolioModel?> GetPortfolioByIdAsync(Guid id)
    {
        return await portfolioRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<PortfolioModel>> GetAllByArtistIdAsync(Guid artistId)
    {
        return await portfolioRepository.GetAllByArtistIdAsync(artistId);
    }

    public async Task AddPortfolioAsync(Guid artistId,
        string title,
        string? description,
        Guid techniqueId)
    {
        await portfolioRepository.AddAsync(artistId, title, description, techniqueId);
    }

    public async Task UpdatePortfolioAsync(Guid id,
        Guid artistId,
        string title,
        string? description,
        Guid techniqueId)
    {
        await portfolioRepository.UpdateAsync(id, artistId, title, description, techniqueId);
    }

    public async Task DeletePortfolioAsync(Guid id)
    {
        await portfolioRepository.DeleteAsync(id);
    }
}
