using ArtLink.Domain.Models;
using ArtLink.Domain.Models.Enums;
using AutoFixture;

namespace ArtLink.Tests.Fixtures;

public class ContractFixture
{
    private readonly IFixture _fixture;

    public ContractFixture()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        // Настраиваем даты без времени
        _fixture.Customize<DateTime>(composer => composer.FromFactory(() => DateTime.Today.AddDays(_fixture.Create<int>() % 365)));
    }

    public Contract CreateContract(
        Guid? id = null,
        Guid? artistId = null,
        Guid? employerId = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ContractState? state = null)
    {
        var contract = _fixture.Build<Contract>()
            .With(c => c.Id, id ?? Guid.NewGuid())
            .With(c => c.ArtistId, artistId ?? Guid.NewGuid())
            .With(c => c.EmployerId, employerId ?? Guid.NewGuid())
            .With(c => c.StartDate, startDate ?? DateTime.Today)
            .With(c => c.EndDate, endDate ?? DateTime.Today.AddDays(7))
            .With(c => c.Status, state ?? ContractState.Draft)
            .With(c => c.ProjectDescription, description ?? "interesting")
            .Create();

        return contract;
    }

    public List<Contract> CreateContracts(int count, Guid? artistId = null, Guid? employerId = null, ContractState? state = null)
    {
        var contracts = new List<Contract>();
        for (int i = 0; i < count; i++)
        {
            contracts.Add(CreateContract(
                artistId: artistId,
                employerId: employerId,
                state: state,
                description: $"Project {i + 1}"));
        }
        return contracts;
    }

    public (Guid artistId, Guid employerId, List<Contract> artistContracts, List<Contract> employerContracts) CreateMixedContracts(int artistCount, int employerCount)
    {
        var artistId = Guid.NewGuid();
        var employerId = Guid.NewGuid();
        
        var artistContracts = CreateContracts(artistCount, artistId: artistId);
        var employerContracts = CreateContracts(employerCount, employerId: employerId);

        return (artistId, employerId, artistContracts, employerContracts);
    }
}