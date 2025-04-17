using EmployerModel = ArtLink.Domain.Models.Employer;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ArtLink.Services.Employer;

public class EmployerService(IEmployerRepository employerRepository, ILogger<EmployerService> logger) : IEmployerService
{
    public async Task<EmployerModel?> GetEmployerByIdAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[EmployerService][GetById] Attempting to get employer with ID: {Id}", id);
            var employer = await employerRepository.GetByIdAsync(id);
            if (employer == null)
            {
                logger.LogWarning("[EmployerService][GetById] Employer not found with ID: {Id}", id);
            }
            return employer;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerService][GetById] Error getting employer with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<EmployerModel>> GetAllEmployersAsync()
    {
        try
        {
            logger.LogInformation("[EmployerService][GetAll] Attempting to get all employers.");
            var employers = (await employerRepository.GetAllEmployersAsync()).ToList();
            logger.LogInformation("[EmployerService][GetAll] Found {Count} employers.", employers.Count);
            return employers;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerService][GetAll] Error fetching employers.");
            throw;
        }
    }

    public async Task AddEmployerAsync(
        string companyName,
        string email,
        string passwordHash,
        string cpFirstName,
        string cpLastName)
    {
        try
        {
            logger.LogInformation("[EmployerService][Add] Adding new employer: {CompanyName}", companyName);
            await employerRepository.AddAsync(companyName, email, passwordHash, cpFirstName, cpLastName);
            logger.LogInformation("[EmployerService][Add] Successfully added employer: {CompanyName}", companyName);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerService][Add] Error adding employer: {CompanyName}", companyName);
            throw;
        }
    }

    public async Task UpdateEmployerAsync(
        Guid id,
        string companyName,
        string email,
        string cpFirstName,
        string cpLastName)
    {
        try
        {
            logger.LogInformation("[EmployerService][Update] Updating employer with ID: {Id}", id);
            await employerRepository.UpdateAsync(id, companyName, email, cpFirstName, cpLastName);
            logger.LogInformation("[EmployerService][Update] Successfully updated employer with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerService][Update] Error updating employer with ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteEmployerAsync(Guid id)
    {
        try
        {
            logger.LogInformation("[EmployerService][Delete] Deleting employer with ID: {Id}", id);
            await employerRepository.DeleteAsync(id);
            logger.LogInformation("[EmployerService][Delete] Successfully deleted employer with ID: {Id}", id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerService][Delete] Error deleting employer with ID: {Id}", id);
            throw;
        }
    }

    public async Task<EmployerModel?> LoginEmployerAsync(string email, string passwordHash)
    {
        try
        {
            logger.LogInformation("[EmployerService][Login] Attempting to log in employer with email: {Email}", email);
            var employer = await employerRepository.LoginAsync(email, passwordHash);
            if (employer == null)
            {
                logger.LogWarning("[EmployerService][Login] Invalid login attempt for email: {Email}", email);
            }
            return employer;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerService][Login] Error logging in employer with email: {Email}", email);
            throw;
        }
    }
}
