using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Converters;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtLink.DataAccess.Repositories;

public class EmployerRepository : IEmployerRepository
{
    private readonly ArtLinkDbContext _context;

    public EmployerRepository(ArtLinkDbContext context)
    {
        _context = context;
    }

    public async Task<Employer?> GetByIdAsync(Guid id)
    {
        var employerDb = await _context.Employers
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
        return employerDb?.ToDomain();
    }

    public async Task<IEnumerable<Employer>> GetAllEmployersAsync()
    {
        var employersDb = await _context.Employers
            .AsNoTracking()
            .ToListAsync();
        return employersDb.Select(e => e.ToDomain());
    }

    public async Task AddAsync(string companyName, 
        string email, 
        string cpFirstName, 
        string cpLastName,
        string passwordHash)
    {
        var employerDb = new EmployerDb(
            id: Guid.NewGuid(),
            companyName: companyName,
            email: email,
            passwordHash: passwordHash,
            cpFirstName: cpFirstName,
            cpLastName: cpLastName
        );

        await _context.Employers.AddAsync(employerDb);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, 
        string companyName, 
        string email, 
        string cpFirstName, 
        string cpLastName)
    {
        var employerDb = await _context.Employers.FindAsync(id);
        if (employerDb != null)
        {
            employerDb.CompanyName = companyName;
            employerDb.Email = email;
            employerDb.CpFirstName = cpFirstName;
            employerDb.CpLastName = cpLastName;

            _context.Employers.Update(employerDb);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var employerDb = await _context.Employers.FindAsync(id);
        if (employerDb != null)
        {
            _context.Employers.Remove(employerDb);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Employer>> SearchByPromptAsync(string prompt)
    {
        var employersDb = await _context.Employers
            .AsNoTracking()
            .Where(e => e.CompanyName.Contains(prompt) || e.CpFirstName.Contains(prompt) || e.CpLastName.Contains(prompt))
            .ToListAsync();
        return employersDb.Select(e => e.ToDomain());
    }
}

