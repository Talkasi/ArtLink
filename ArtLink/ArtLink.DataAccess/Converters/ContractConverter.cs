using System.Diagnostics.CodeAnalysis;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Models;
using ArtLink.DataAccess.Models.Enums;

namespace ArtLink.DataAccess.Converters;

public static class ContractConverter
{
    [return: NotNullIfNotNull(nameof(contractDb))]
    public static Contract? ToDomain(this ContractDb? contractDb)
    {
        if (contractDb is null)
            return null;

        return new Contract(
            id: contractDb.Id,
            artistId: contractDb.ArtistId,
            employerId: contractDb.EmployerId,
            projectDescription: contractDb.ProjectDescription,
            startDate: contractDb.StartDate,
            endDate: contractDb.EndDate,
            status: contractDb.Status.ToDomain()
        );
    }
    
    [return: NotNullIfNotNull(nameof(contract))]
    public static ContractDb? ToDataAccess(this Contract? contract)
    {
        if (contract is null)
            return null;

        return new ContractDb(
            id: contract.Id,
            artistId: contract.ArtistId,
            employerId: contract.EmployerId,
            projectDescription: contract.ProjectDescription,
            startDate: contract.StartDate,
            endDate: contract.EndDate,
            status: contract.Status.ToDb()
        );
    }
}