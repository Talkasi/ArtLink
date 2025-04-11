using ArtLink.DataAccess.Models.Enums;
using ArtLink.Domain.Models.Enums;

namespace ArtLink.DataAccess.Converters;

public static class ContractStatesConverter
{
    public static ContractState ToDomain(this ContractStateDb contractStateDb)
    {
        return contractStateDb switch
        {
            ContractStateDb.Draft => ContractState.Draft,
            ContractStateDb.Send => ContractState.Send,
            ContractStateDb.Read => ContractState.Read,
            ContractStateDb.Accepted => ContractState.Accepted,
            ContractStateDb.Rejected => ContractState.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(contractStateDb), contractStateDb, null)
        };
    }

    public static ContractStateDb ToDb(this ContractState contractState)
    {
        return contractState switch
        {
            ContractState.Draft => ContractStateDb.Draft,
            ContractState.Send => ContractStateDb.Send,
            ContractState.Read => ContractStateDb.Read,
            ContractState.Accepted => ContractStateDb.Accepted,
            ContractState.Rejected => ContractStateDb.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(contractState), contractState, null)
        };
    }

    public static ContractStateDb ToDb(this ContractState? contractState)
    {
        return contractState switch
        {
            ContractState.Draft => ContractStateDb.Draft,
            ContractState.Send => ContractStateDb.Send,
            ContractState.Read => ContractStateDb.Read,
            ContractState.Accepted => ContractStateDb.Accepted,
            ContractState.Rejected => ContractStateDb.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(contractState), contractState, null)
        };
    }
}

