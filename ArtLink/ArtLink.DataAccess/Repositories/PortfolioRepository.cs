using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtLink.DataAccess.Repositories;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly ArtLinkDbContext _context;

    public PortfolioRepository(ArtLinkDbContext context)
    {
        _context = context;
    }

    public async Task<Portfolio?> GetByIdAsync(Guid id)
    {
        var portfolioDb = await _context.Portfolios
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
        return portfolioDb?.ToDomain();
    }

    public async Task<IEnumerable<Portfolio>> GetAllByArtistIdAsync(Guid artistId)
    {
        var portfoliosDb = await _context.Portfolios
            .AsNoTracking()
            .Where(p => p.ArtistId == artistId)
            .ToListAsync();
        return portfoliosDb.Select(p => p.ToDomain());
    }

    public async Task AddAsync(Guid artistId, string title, string? description, Guid techniqueId)
    {
        var portfolio = new PortfolioDb(Guid.NewGuid(), artistId, techniqueId, title, description);
        await _context.Portfolios.AddAsync(portfolio);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, Guid artistId, string title, string? description, Guid techniqueId)
    {
        var portfolioDb = await _context.Portfolios.FindAsync(id);
        if (portfolioDb != null)
        {
            portfolioDb.ArtistId = artistId;
            portfolioDb.Title = title;
            portfolioDb.Description = description;
            portfolioDb.TechniqueId = techniqueId;

            _context.Portfolios.Update(portfolioDb);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var portfolioDb = await _context.Portfolios.FindAsync(id);
        if (portfolioDb != null)
        {
            _context.Portfolios.Remove(portfolioDb);
            await _context.SaveChangesAsync();
        }
    }
}

