using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using TechniqueModel = ArtLink.Domain.Models.Technique;

namespace ArtLink.Services.Technique;

public class TechniqueService(ITechniqueRepository techniqueRepository, ILogger<TechniqueService> logger) : ITechniqueService
{
    public async Task<IEnumerable<TechniqueModel>> GetAllAsync()
    {
        try
        {
            logger.LogInformation("[TechniqueService][GetAllAsync] Fetching all techniques");
            var techniques = (await techniqueRepository.GetAllAsync()).ToList();
            logger.LogInformation("[TechniqueService][GetAllAsync] Found {Count} techniques", techniques.Count);
            return techniques;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[TechniqueService][GetAllAsync] Error fetching all techniques");
            throw;
        }
    }

    public async Task<Guid> AddTechniqueAsync(string name, string description)
    {
        try
        {
            logger.LogInformation("[TechniqueService][AddTechniqueAsync] Adding new technique: {Name}", name);
            var id = await techniqueRepository.AddAsync(name, description);
            logger.LogInformation("[TechniqueService][AddTechniqueAsync] Successfully added technique: {Name}", name);
            return id;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[TechniqueService][AddTechniqueAsync] Error adding technique: {Name}", name);
            throw;
        }
    }

    public async Task UpdateTechniqueAsync(Guid id, string name, string description)
    {
        try
        {
            logger.LogInformation("[TechniqueService][UpdateTechniqueAsync] Updating technique with ID: {Id}", id);
            await techniqueRepository.UpdateAsync(id, name, description);
            logger.LogInformation("[TechniqueService][UpdateTechniqueAsync] Successfully updated technique with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[TechniqueService][UpdateTechniqueAsync] Error updating technique with ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteTechniqueAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[TechniqueService][DeleteTechniqueAsync] Deleting technique with ID: {Id}", id);
            await techniqueRepository.DeleteAsync(id);
            logger.LogInformation("[TechniqueService][DeleteTechniqueAsync] Successfully deleted technique with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[TechniqueService][DeleteTechniqueAsync] Error deleting technique with ID: {Id}", id);
            throw;
        }
    }
}
