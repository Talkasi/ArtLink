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
    private const string ClassName = nameof(ArtistRepository);

    public async Task<Artist?> GetByIdAsync(Guid id)
    {
        const string method = nameof(GetByIdAsync);
        try
        {
            var artistDb = await context.Artists
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artistDb != null)
                logger.LogInformation("[{Class}][{Method}] Retrieved artist with ID {ArtistId}", ClassName, method, id);
            else
                logger.LogWarning("[{Class}][{Method}] Artist with ID {ArtistId} not found.", ClassName, method, id);

            return artistDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get artist by id {ArtistId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task<IEnumerable<Artist>> GetAllAsync()
    {
        const string method = nameof(GetAllAsync);
        try
        {
            var artistsDb = await context.Artists
                .AsNoTracking()
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Retrieved all artists. Count: {Count}", ClassName, method, artistsDb.Count);

            return artistsDb.Select(a => a.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get all artists.", ClassName, method);
            throw;
        }
    }

    public async Task<Guid> AddAsync(string firstName,
        string lastName,
        string email,
        string passwordHash,
        string? bio,
        string? profilePicturePath,
        int? experience)
    {
        const string method = nameof(AddAsync);
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

            logger.LogInformation("[{Class}][{Method}] Added new artist: {Email}", ClassName, method, email);
            return artistDb.Id;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to add artist {Email}.", ClassName, method, email);
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
        const string method = nameof(UpdateAsync);
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

                logger.LogInformation("[{Class}][{Method}] Updated artist with ID {ArtistId}", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Artist with ID {ArtistId} not found for update.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to update artist {ArtistId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        const string method = nameof(DeleteAsync);
        try
        {
            var artistDb = await context.Artists.FindAsync(id);
            if (artistDb != null)
            {
                context.Artists.Remove(artistDb);
                await context.SaveChangesAsync();

                logger.LogInformation("[{Class}][{Method}] Deleted artist with ID {ArtistId}", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Artist with ID {ArtistId} not found for deletion.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to delete artist {ArtistId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task<IEnumerable<Artist>> SearchByPromptAsync(string prompt)
    {
        const string method = nameof(SearchByPromptAsync);
        try
        {
            var artistsDb = await context.Artists
                .AsNoTracking()
                .Where(a => a.FirstName.Contains(prompt) || a.LastName.Contains(prompt) || a.Email.Contains(prompt))
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Searched artists by prompt '{Prompt}', found: {Count}", ClassName, method, prompt, artistsDb.Count);

            return artistsDb.Select(a => a.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to search artists with prompt '{Prompt}'.", ClassName, method, prompt);
            throw;
        }
    }

    public async Task<Artist?> LoginAsync(string email, string passwordHash)
    {
        const string method = nameof(LoginAsync);
        try
        {
            var artist = await context.Artists
                .FirstOrDefaultAsync(a => a.Email == email && a.PasswordHash == passwordHash);

            if (artist != null)
                logger.LogInformation("[{Class}][{Method}] Artist login successful for {Email}", ClassName, method, email);
            else
                logger.LogWarning("[{Class}][{Method}] Artist login failed for {Email}", ClassName, method, email);

            return artist?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to login artist with email {Email}.", ClassName, method, email);
            throw;
        }
    }
}
