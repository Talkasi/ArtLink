using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Interfaces.Services;
using TechniqueModel = ArtLink.Domain.Models.Technique;

namespace ArtLink.Services.Technique;

public class TechniqueService(ITechniqueRepository techniqueRepository) : ITechniqueService
{
    public async Task<IEnumerable<TechniqueModel>> GetAllAsync()
    {
        return await techniqueRepository.GetAllAsync();
    }

    public async Task AddTechniqueAsync(string name, string description)
    {
        await techniqueRepository.AddAsync(name, description);
    }

    public async Task UpdateTechniqueAsync(Guid id, string name, string description)
    {
        await techniqueRepository.UpdateAsync(id, name, description);
    }

    public async Task DeleteTechniqueAsync(Guid id)
    {
        await techniqueRepository.DeleteAsync(id);
    }
}
