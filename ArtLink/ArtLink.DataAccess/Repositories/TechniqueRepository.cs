using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArtLink.DataAccess.Repositories;

public class TechniqueRepository(ArtLinkDbContext context, ILogger<TechniqueRepository> logger) : ITechniqueRepository
{
    private const string ClassName = nameof(TechniqueRepository);

    public async Task<IEnumerable<Technique>> GetAllAsync()
    {
        const string method = nameof(GetAllAsync);
        try
        {
            var techniques = await context.Techniques
                .AsNoTracking()
                .Select(t => new Technique(t.Id, t.Name, t.Description))
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Retrieved {Count} techniques.", ClassName, method, techniques.Count);
            return techniques;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to retrieve all techniques.", ClassName, method);
            throw;
        }
    }

    public async Task AddAsync(string name, string description)
    {
        const string method = nameof(AddAsync);
        try
        {
            var technique = new TechniqueDb(Guid.NewGuid(), name, description);
            await context.Techniques.AddAsync(technique);
            await context.SaveChangesAsync();

            logger.LogInformation("[{Class}][{Method}] Added technique '{Name}'.", ClassName, method, name);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to add technique '{Name}'.", ClassName, method, name);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id, string name, string description)
    {
        const string method = nameof(UpdateAsync);
        try
        {
            var technique = await context.Techniques.FindAsync(id);
            if (technique is null)
            {
                logger.LogWarning("[{Class}][{Method}] Technique with ID {Id} not found for update.", ClassName, method, id);
                return;
            }

            technique.Name = name;
            technique.Description = description;
            await context.SaveChangesAsync();

            logger.LogInformation("[{Class}][{Method}] Updated technique with ID {Id}.", ClassName, method, id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to update technique {Id}.", ClassName, method, id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        const string method = nameof(DeleteAsync);
        try
        {
            var technique = await context.Techniques.FindAsync(id);
            if (technique is not null)
            {
                context.Techniques.Remove(technique);
                await context.SaveChangesAsync();

                logger.LogInformation("[{Class}][{Method}] Deleted technique with ID {Id}.", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Technique with ID {Id} not found for deletion.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to delete technique {Id}.", ClassName, method, id);
            throw;
        }
    }
}
