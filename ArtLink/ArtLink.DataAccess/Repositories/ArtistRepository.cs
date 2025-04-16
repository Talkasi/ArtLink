using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArtLink.DataAccess.Repositories;

public class ArtistRepository(ArtLinkDbContext context, ILogger<ArtistRepository> logger) : IArtistRepository
{
    public async Task<Artist?> GetByIdAsync(Guid id)
    {
        try
        {
            var artistDb = await context.Artists
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
            return artistDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get artist by id {artistId}.", id);
            throw;
        }
    }

    public async Task<IEnumerable<Artist>> GetAllAsync()
    {
        try
        {
            var artistsDb = await context.Artists
                .AsNoTracking()
                .ToListAsync();
            return artistsDb.Select(a => a.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get all artists.");
            throw;
        }
    }

    public async Task AddAsync(string firstName,
        string lastName,
        string email,
        string passwordHash,
        string? bio,
        string? profilePicturePath,
        int? experience)
    {
        try
        {
            var artistDb = new ArtistDb(
                id: Guid.NewGuid(),
                firstName: firstName,
                lastName: lastName,
                email: email,
                passwordHash: passwordHash,
                bio: bio,
                profilePicturePath: profilePicturePath,
                experience: experience);

            await context.Artists.AddAsync(artistDb);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add artist {email}.", email);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id,
        string firstName,
        string lastName,
        string email,
        string? bio,
        string? profilePicturePath,
        int? experience)
    {
        try
        {
            var artistDb = await context.Artists.FindAsync(id);
            if (artistDb != null)
            {
                artistDb.FirstName = firstName;
                artistDb.LastName = lastName;
                artistDb.Email = email;
                artistDb.Bio = bio;
                artistDb.ProfilePicturePath = profilePicturePath;
                artistDb.Experience = experience;

                context.Artists.Update(artistDb);
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogWarning("Artist with id {artistId} not found for update.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update artist {artistId}.", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var artistDb = await context.Artists.FindAsync(id);
            if (artistDb != null)
            {
                context.Artists.Remove(artistDb);
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogWarning("Artist with id {artistId} not found for deletion.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete artist {artistId}.", id);
            throw;
        }
    }

    public async Task<IEnumerable<Artist>> SearchByPromptAsync(string prompt)
    {
        try
        {
            var artistsDb = await context.Artists
                .AsNoTracking()
                .Where(a => a.FirstName.Contains(prompt) || a.LastName.Contains(prompt) || a.Email.Contains(prompt))
                .ToListAsync();
            return artistsDb.Select(a => a.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to search artists with prompt '{prompt}'.", prompt);
            throw;
        }
    }

    public async Task<Artist?> LoginAsync(string email, string passwordHash)
    {
        try
        {
            var artist = await context.Artists
                .FirstOrDefaultAsync(a => a.Email == email && a.PasswordHash == passwordHash);
            return artist?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to login artist with email {email} and given password.", email);
            throw;
        }
    }
}
