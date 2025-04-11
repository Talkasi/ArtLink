using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtLink.DataAccess.Repositories;

public class ArtistRepository : IArtistRepository
{
    private readonly ArtLinkDbContext _context;

    public ArtistRepository(ArtLinkDbContext context)
    {
        _context = context;
    }

    public async Task<Artist?> GetByIdAsync(Guid id)
    {
        var artistDb = await _context.Artists
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
        return artistDb?.ToDomain(); // Используем конвертер
    }

    public async Task<IEnumerable<Artist>> GetAllAsync()
    {
        var artistsDb = await _context.Artists
            .AsNoTracking()
            .ToListAsync();
        return artistsDb.Select(a => a.ToDomain()); // Используем конвертер
    }

    public async Task AddAsync(string firstName, 
        string lastName, 
        string email,
        string passwordHash,
        string? bio, 
        string? profilePicturePath, 
        int? experience)
    {
        var artistDb = new ArtistDb(
            id: Guid.NewGuid(),
            firstName: firstName,
            lastName: lastName,
            email: email,
            passwordHash: passwordHash,
            bio: bio,
            profilePicturePath: profilePicturePath,
            experience: experience
        );

        await _context.Artists.AddAsync(artistDb);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, 
        string firstName, 
        string lastName, 
        string email, 
        string? bio, 
        string? profilePicturePath, 
        int? experience)
    {
        var artistDb = await _context.Artists.FindAsync(id);
        if (artistDb != null)
        {
            artistDb.FirstName = firstName;
            artistDb.LastName = lastName;
            artistDb.Email = email;
            artistDb.Bio = bio;
            artistDb.ProfilePicturePath = profilePicturePath;
            artistDb.Experience = experience;

            _context.Artists.Update(artistDb);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var artistDb = await _context.Artists.FindAsync(id);
        if (artistDb != null)
        {
            _context.Artists.Remove(artistDb);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Artist>> SearchByPromptAsync(string prompt)
    {
        var artistsDb = await _context.Artists
            .AsNoTracking()
            .Where(a => a.FirstName.Contains(prompt) || a.LastName.Contains(prompt) || a.Email.Contains(prompt))
            .ToListAsync();
        return artistsDb.Select(a => a.ToDomain());
    }
}

