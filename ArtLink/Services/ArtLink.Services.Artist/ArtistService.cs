using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using ArtistModel = ArtLink.Domain.Models.Artist;

namespace ArtLink.Services.Artist;

public class ArtistService(IArtistRepository artistRepository, ILogger<ArtistService> logger) : IArtistService
{
    public async Task<ArtistModel?> GetArtistByIdAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[ArtistService][GetById] Trying to get artist with ID: {Id}", id);
            var artist = await artistRepository.GetByIdAsync(id);
            if (artist == null)
            {
                logger.LogWarning("[ArtistService][GetById] Artist not found: {Id}", id);
            }
            return artist;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistService][GetById] Error getting artist with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ArtistModel>> GetAllArtistsAsync()
    {
        try
        {
            logger.LogInformation("[ArtistService][GetAll] Fetching all artists");
            var artists = (await artistRepository.GetAllAsync()).ToList();
            logger.LogInformation("[ArtistService][GetAll] Found {Count} artists", artists.Count);
            return artists;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistService][GetAll] Error fetching artists");
            throw;
        }
    }

    public async Task<Guid> AddArtistAsync(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        string? bio,
        string? profilePicturePath,
        int? experience)
    {
        try
        {
            logger.LogInformation("[ArtistService][Add] Adding new artist: {FirstName} {LastName}", firstName, lastName);
            var id = await artistRepository.AddAsync(
                firstName,
                lastName,
                email,
                passwordHash,
                bio,
                profilePicturePath,
                experience
            );
            logger.LogInformation("[ArtistService][Add] Successfully added artist: {FirstName} {LastName}", firstName, lastName);
            return id;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistService][Add] Error adding artist: {FirstName} {LastName}", firstName, lastName);
            throw;
        }
    }

    public async Task UpdateArtistAsync(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string? bio,
        string? profilePicturePath,
        int? experience)
    {
        try
        {
            logger.LogInformation("[ArtistService][Update] Updating artist with ID: {Id}", id);
            await artistRepository.UpdateAsync(
                id,
                firstName,
                lastName,
                email,
                bio,
                profilePicturePath,
                experience
            );
            logger.LogInformation("[ArtistService][Update] Successfully updated artist with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistService][Update] Error updating artist with ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteArtistAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[ArtistService][Delete] Deleting artist with ID: {Id}", id);
            await artistRepository.DeleteAsync(id);
            logger.LogInformation("[ArtistService][Delete] Successfully deleted artist with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistService][Delete] Error deleting artist with ID: {Id}", id);
            throw;
        }
    }

    public async Task<ArtistModel?> LoginArtistAsync(string email, string passwordHash)
    {
        try
        {
            logger.LogInformation("[ArtistService][Login] Attempting login for artist with email: {Email}", email);
            var artist = await artistRepository.LoginAsync(email, passwordHash);
            if (artist == null)
            {
                logger.LogWarning("[ArtistService][Login] Invalid credentials for artist with email: {Email}", email);
            }
            return artist;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistService][Login] Error during login for artist with email: {Email}", email);
            throw;
        }
    }
}
