using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArtLink.DataAccess.Repositories;

public class EmployerRepository(ArtLinkDbContext context, ILogger<EmployerRepository> logger) : IEmployerRepository
{
    private const string ClassName = nameof(EmployerRepository);

    public async Task<Employer?> GetByIdAsync(Guid id)
    {
        const string method = nameof(GetByIdAsync);
        try
        {
            var employerDb = await context.Employers
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employerDb != null)
                logger.LogInformation("[{Class}][{Method}] Retrieved employer with ID {EmployerId}.", ClassName, method, id);
            else
                logger.LogWarning("[{Class}][{Method}] Employer with ID {EmployerId} not found.", ClassName, method, id);

            return employerDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get employer by ID {EmployerId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task<IEnumerable<Employer>> GetAllEmployersAsync()
    {
        const string method = nameof(GetAllEmployersAsync);
        try
        {
            var employersDb = await context.Employers
                .AsNoTracking()
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Retrieved {Count} employers.", ClassName, method, employersDb.Count);

            return employersDb.Select(e => e.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to get all employers.", ClassName, method);
            throw;
        }
    }

    public async Task AddAsync(string companyName,
        string email,
        string passwordHash,
        string cpFirstName,
        string cpLastName)
    {
        const string method = nameof(AddAsync);
        try
        {
            var employerDb = new EmployerDb(
                id: Guid.NewGuid(),
                companyName: companyName,
                email: email,
                passwordHash: passwordHash,
                cpFirstName: cpFirstName,
                cpLastName: cpLastName);

            await context.Employers.AddAsync(employerDb);
            await context.SaveChangesAsync();

            logger.LogInformation("[{Class}][{Method}] Added employer with email {Email}.", ClassName, method, email);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to add employer {Email}.", ClassName, method, email);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id,
        string companyName,
        string email,
        string cpFirstName,
        string cpLastName)
    {
        const string method = nameof(UpdateAsync);
        try
        {
            var employerDb = await context.Employers.FindAsync(id);
            if (employerDb != null)
            {
                employerDb.CompanyName = companyName;
                employerDb.Email = email;
                employerDb.CpFirstName = cpFirstName;
                employerDb.CpLastName = cpLastName;

                context.Employers.Update(employerDb);
                await context.SaveChangesAsync();

                logger.LogInformation("[{Class}][{Method}] Updated employer with ID {EmployerId}.", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Employer with ID {EmployerId} not found for update.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to update employer {EmployerId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        const string method = nameof(DeleteAsync);
        try
        {
            var employerDb = await context.Employers.FindAsync(id);
            if (employerDb != null)
            {
                context.Employers.Remove(employerDb);
                await context.SaveChangesAsync();

                logger.LogInformation("[{Class}][{Method}] Deleted employer with ID {EmployerId}.", ClassName, method, id);
            }
            else
            {
                logger.LogWarning("[{Class}][{Method}] Employer with ID {EmployerId} not found for deletion.", ClassName, method, id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to delete employer {EmployerId}.", ClassName, method, id);
            throw;
        }
    }

    public async Task<IEnumerable<Employer>> SearchByPromptAsync(string prompt)
    {
        const string method = nameof(SearchByPromptAsync);
        try
        {
            var employersDb = await context.Employers
                .AsNoTracking()
                .Where(e => e.CompanyName.Contains(prompt) || e.CpFirstName.Contains(prompt) || e.CpLastName.Contains(prompt))
                .ToListAsync();

            logger.LogInformation("[{Class}][{Method}] Found {Count} employers with prompt '{Prompt}'.", ClassName, method, employersDb.Count, prompt);

            return employersDb.Select(e => e.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to search employers with prompt '{Prompt}'.", ClassName, method, prompt);
            throw;
        }
    }

    public async Task<Employer?> LoginAsync(string email, string passwordHash)
    {
        const string method = nameof(LoginAsync);
        try
        {
            var employer = await context.Employers
                .FirstOrDefaultAsync(e => e.Email == email && e.PasswordHash == passwordHash);

            if (employer != null)
                logger.LogInformation("[{Class}][{Method}] Login successful for email {Email}.", ClassName, method, email);
            else
                logger.LogWarning("[{Class}][{Method}] Login failed for email {Email}.", ClassName, method, email);

            return employer?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Class}][{Method}] Failed to login employer with email {Email}.", ClassName, method, email);
            throw;
        }
    }
}
