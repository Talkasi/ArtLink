using ArtistModel = ArtLink.Domain.Models.Artist;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Interfaces.Repositories;

namespace ArtLink.Services.Artist;

public class ArtistService(IArtistRepository artistRepository) : IArtistService
{
    public async Task<ArtistModel?> GetArtistByIdAsync(Guid id)
    {
        return await artistRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<ArtistModel>> GetAllArtistsAsync()
    {
        return await artistRepository.GetAllAsync();
    }

    public async Task AddArtistAsync(string firstName,
        string lastName,
        string email,
        string bio,
        string profilePicturePath,
        int experience)
    {
        await artistRepository.AddAsync(firstName, lastName, email, bio, profilePicturePath, experience);
    }

    public async Task UpdateArtistAsync(Guid id,
        string firstName,
        string lastName,
        string email,
        string bio,
        string profilePicturePath,
        int experience)
    {
        await artistRepository.UpdateAsync(id, firstName, lastName, email, bio, profilePicturePath, experience);
    }

    public async Task DeleteArtistAsync(Guid id)
    {
        await artistRepository.DeleteAsync(id);
    }
}
