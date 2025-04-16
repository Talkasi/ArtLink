using ArtworkModel = ArtLink.Domain.Models.Artwork;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Interfaces.Repositories;

namespace ArtLink.Services.Artwork;

public class ArtworkService(IArtworkRepository artworkRepository) : IArtworkService
{
    public async Task<ArtworkModel?> GetArtworkByIdAsync(Guid id)
    {
        return await artworkRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<ArtworkModel>> GetAllByPortfolioIdAsync(Guid portfolioId)
    {
        return await artworkRepository.GetAllByPortfolioIdAsync(portfolioId);
    }

    public async Task AddArtworkAsync(Guid portfolioId,
        string title,
        string imagePath,
        string? description)
    {
        await artworkRepository.AddAsync(portfolioId, title, imagePath, description);
    }

    public async Task UpdateArtworkAsync(Guid id,
        Guid portfolioId,
        string title,
        string imagePath,
        string? description)
    {
        await artworkRepository.UpdateAsync(id, portfolioId, title, imagePath, description);
    }

    public async Task DeleteArtworkAsync(Guid id)
    {
        await artworkRepository.DeleteAsync(id);
    }
}
