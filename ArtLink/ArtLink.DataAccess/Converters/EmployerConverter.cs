using System.Diagnostics.CodeAnalysis;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Models;

namespace ArtLink.DataAccess.Converters;

public static class EmployerConverter
{
    [return: NotNullIfNotNull(nameof(employerDb))]
    public static Employer? ToDomain(this EmployerDb? employerDb)
    {
        if (employerDb is null)
            return null;

        return new Employer(
            id: employerDb.Id,
            companyName: employerDb.CompanyName,
            email: employerDb.Email,
            passwordHash: employerDb.PasswordHash,
            cpFirstName: employerDb.CpFirstName,
            cpLastName: employerDb.CpLastName
        );
    }

    [return: NotNullIfNotNull(nameof(employer))]
    public static EmployerDb? ToDataAccess(this Employer? employer)
    {
        if (employer is null)
            return null;

        return new EmployerDb(
            id: employer.Id,
            companyName: employer.CompanyName,
            email: employer.Email,
            passwordHash: employer.PasswordHash,
            cpFirstName: employer.CpFirstName,
            cpLastName: employer.CpLastName
        );
    }
}

