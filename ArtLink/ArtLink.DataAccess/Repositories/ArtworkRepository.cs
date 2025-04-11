using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtLink.DataAccess.Repositories;

public class ArtworkRepository : IArtworkRepository
{
    private readonly ArtLinkDbContext _context;

    public ArtworkRepository(ArtLinkDbContext context)
    {
        _context = context;
    }

    public async Task<Artwork?> GetByIdAsync(Guid id)
    {
        var artworkDb = await _context.Artworks
            .FirstOrDefaultAsync(a => a.Id == id);
        return artworkDb?.ToDomain();
    }

    public async Task<IEnumerable<Artwork>> GetAllByPortfolioIdAsync(Guid portfolioId)
    {
        var artworksDb = await _context.Artworks
            .Where(a => a.PortfolioId == portfolioId)
            .ToListAsync();
        return artworksDb.Select(a => a.ToDomain());
    }

    public async Task AddAsync(Guid portfolioId, string title, string description, string imagePath)
    {
        var artworkDb = new ArtworkDb(Guid.NewGuid(), portfolioId, title, description, imagePath);
        await _context.Artworks.AddAsync(artworkDb);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, Guid portfolioId, string title, string description, string imagePath)
    {
        var artwork = await _context.Artworks.FindAsync(id);
        if (artwork != null)
        {
            artwork.PortfolioId = portfolioId;
            artwork.Title = title;
            artwork.Description = description;
            artwork.ImagePath = imagePath;

            await _context.SaveChangesAsync();
        }
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var artwork = await _context.Artworks.FindAsync(id);
        if (artwork != null)
        {
            _context.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();
        }
    }


    public async Task<IEnumerable<Artwork>> SearchByPromptAsync(string prompt)
    {
        var artworksDb = await _context.Artworks
            .Where(a => a.Title.Contains(prompt) || 
                         (a.Description != null && a.Description.Contains(prompt)))
            .ToListAsync();
        return artworksDb.Select(a => a.ToDomain());
    }
}
