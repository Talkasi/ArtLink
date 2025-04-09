using EmployerModel = ArtLink.Domain.Models.Employer;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Interfaces.Repositories;

namespace ArtLink.Services.Employer;

public class EmployerService(IEmployerRepository employerRepository) : IEmployerService
{
    public async Task<EmployerModel?> GetEmployerByIdAsync(Guid id)
    {
        return await employerRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<EmployerModel>> GetAllEmployersAsync()
    {
        return await employerRepository.GetAllEmployersAsync();
    }

    public async Task AddEmployerAsync(string companyName,
        string email,
        string cpFirstName,
        string cpLastName)
    {
        await employerRepository.AddAsync(companyName, email, cpFirstName, cpLastName);
    }

    public async Task UpdateEmployerAsync(Guid id,
        string companyName,
        string email,
        string cpFirstName,
        string cpLastName)
    {
        await employerRepository.UpdateAsync(id, companyName, email, cpFirstName, cpLastName);
    }

    public async Task DeleteEmployerAsync(Guid id)
    {
        await employerRepository.DeleteAsync(id);
    }
}
