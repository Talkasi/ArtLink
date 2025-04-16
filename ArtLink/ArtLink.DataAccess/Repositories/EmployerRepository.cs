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
    public async Task<Employer?> GetByIdAsync(Guid id)
    {
        try
        {
            var employerDb = await context.Employers
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
            return employerDb?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get employer by id {employerId}.", id);
            throw;
        }
    }

    public async Task<IEnumerable<Employer>> GetAllEmployersAsync()
    {
        try
        {
            var employersDb = await context.Employers
                .AsNoTracking()
                .ToListAsync();
            return employersDb.Select(e => e.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get all employers.");
            throw;
        }
    }

    public async Task AddAsync(string companyName,
        string email,
        string passwordHash,
        string cpFirstName,
        string cpLastName)
    {
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
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add employer {email}.", email);
            throw;
        }
    }

    public async Task UpdateAsync(Guid id,
        string companyName,
        string email,
        string cpFirstName,
        string cpLastName)
    {
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
            }
            else
            {
                logger.LogWarning("Employer with id {employerId} not found for update.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update employer {employerId}.", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var employerDb = await context.Employers.FindAsync(id);
            if (employerDb != null)
            {
                context.Employers.Remove(employerDb);
                await context.SaveChangesAsync();
            }
            else
            {
                logger.LogWarning("Employer with id {employerId} not found for deletion.", id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete employer {employerId}.", id);
            throw;
        }
    }

    public async Task<IEnumerable<Employer>> SearchByPromptAsync(string prompt)
    {
        try
        {
            var employersDb = await context.Employers
                .AsNoTracking()
                .Where(e => e.CompanyName.Contains(prompt) || e.CpFirstName.Contains(prompt) || e.CpLastName.Contains(prompt))
                .ToListAsync();
            return employersDb.Select(e => e.ToDomain());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to search employers with prompt '{prompt}'.", prompt);
            throw;
        }
    }

    public async Task<Employer?> LoginAsync(string email, string passwordHash)
    {
        try
        {
            var employer = await context.Employers
                .FirstOrDefaultAsync(e => e.Email == email && e.PasswordHash == passwordHash);
            return employer?.ToDomain();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to login employer with email {email}.", email);
            throw;
        }
    }
}
