using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArtLink.DataAccess.Repositories;

public class TechniqueRepository(ArtLinkDbContext context, ILogger<TechniqueRepository> logger) : ITechniqueRepository
{
    public async Task<IEnumerable<Technique>> GetAllAsync()
    {
        try
        {
            return await context.Techniques
                .AsNoTracking()
                .Select(t => new Technique(t.Id, t.Name, t.Description))
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving all techniques.");
            throw;
        }
    }

    public async Task AddAsync(string name, string description)
    {
        try
        {
            var technique = new TechniqueDb(Guid.NewGuid(), name, description);
            await context.Techniques.AddAsync(technique);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error adding technique {name}.", name);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id, string name, string description)
    {
        try
        {
            var technique = await context.Techniques.FindAsync(id);
            if (technique is null)
            {
                logger.LogWarning("Technique {id} not found for update.", id);
                return;
            }

            technique.Name = name;
            technique.Description = description;
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating technique {id}.", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var technique = await context.Techniques.FindAsync(id);
            if (technique is not null)
            {
                context.Techniques.Remove(technique);
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogWarning("Technique {id} not found for deletion.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting technique {id}.", id);
            throw;
        }
    }
}