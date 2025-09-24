using Allure.Xunit.Attributes;
using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Domain.Models.Enums;
using ArtLink.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.Repositories;

[AllureSuite("Contract Repository Tests")]
public class ContractRepositoryTests(ContractFixture fixture) : IClassFixture<ContractFixture>
{
    private static ArtLinkDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ArtLinkDbContext(options);
    }

    private static ContractRepository CreateRepository(ArtLinkDbContext context)
    {
        return new ContractRepository(context, NullLogger<ContractRepository>.Instance);
    }

    [Fact]
    [AllureFeature("AddAsync + GetByIdAsync")]
    [AllureStory("Positive case - add and get contract")]
    public async Task AddAndGetById_ShouldReturnCorrectContract()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var contract = fixture.CreateContract(
            description: "Project X", 
            state: ContractState.Accepted);
        
        var id = await repo.AddAsync(
            contract.ArtistId, 
            contract.EmployerId, 
            contract.ProjectDescription, 
            contract.Status, 
            contract.StartDate, 
            contract.EndDate);

        var result = await repo.GetByIdAsync(id);

        result.Should().NotBeNull();
        result.ProjectDescription.Should().Be(contract.ProjectDescription);
        result.Status.Should().Be(contract.Status);
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Positive case - update contract")]
    public async Task Update_ShouldModifyContract()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var contract = fixture.CreateContract(description: "Old Project");
        var contractId = await repo.AddAsync(
            contract.ArtistId, 
            contract.EmployerId, 
            contract.ProjectDescription, 
            contract.Status, 
            contract.StartDate, 
            contract.EndDate);

        var updatedContract = fixture.CreateContract(
            description: "Updated Project", 
            state: ContractState.Accepted);
        
        await repo.UpdateAsync(
            contractId, 
            updatedContract.ArtistId, 
            updatedContract.EmployerId, 
            updatedContract.ProjectDescription, 
            updatedContract.Status, 
            updatedContract.StartDate, 
            updatedContract.EndDate);

        var result = await repo.GetByIdAsync(contractId);

        result.Should().NotBeNull();
        result.ProjectDescription.Should().Be("Updated Project");
        result.Status.Should().Be(ContractState.Accepted);
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Positive case - delete contract")]
    public async Task Delete_ShouldRemoveContract()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var contract = fixture.CreateContract(description: "To Delete");
        var contractId = await repo.AddAsync(
            contract.ArtistId, 
            contract.EmployerId, 
            contract.ProjectDescription, 
            contract.Status, 
            contract.StartDate, 
            contract.EndDate);

        await repo.DeleteAsync(contractId);

        var result = await repo.GetByIdAsync(contractId);
        result.Should().BeNull();
    }
    
    [Fact]
    [AllureFeature("GetByIdAsync")]
    [AllureStory("Negative case - non-existent contract returns null")]
    public async Task GetById_NonExistent_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var contract = await repo.GetByIdAsync(Guid.NewGuid());
        contract.Should().BeNull();
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Negative case - update non-existent contract does not throw")]
    public async Task Update_NonExistent_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var contract = fixture.CreateContract();
        
        Func<Task> act = async () => await repo.UpdateAsync(
            Guid.NewGuid(), 
            contract.ArtistId, 
            contract.EmployerId, 
            "X", 
            ContractState.Accepted,
            contract.StartDate,
            contract.EndDate);
        
        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Negative case - delete non-existent contract does not throw")]
    public async Task Delete_NonExistent_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        Func<Task> act = async () => await repo.DeleteAsync(Guid.NewGuid());
        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("GetByArtistIdAsync")]
    [AllureStory("Positive case - get contracts by artist")]
    public async Task GetByArtistId_ShouldReturnArtistContracts()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var (artistId, _, artistContracts, employerContracts) = fixture.CreateMixedContracts(2, 1);
        
        foreach (var contract in artistContracts.Concat(employerContracts))
        {
            await repo.AddAsync(
                contract.ArtistId,
                contract.EmployerId,
                contract.ProjectDescription,
                contract.Status,
                contract.StartDate,
                contract.EndDate);
        }

        var results = await repo.GetAllByArtistIdAsync(artistId);
        var enumerable = results as Contract[] ?? results.ToArray();
        enumerable.Should().HaveCount(2);
        enumerable.All(c => c.ArtistId == artistId).Should().BeTrue();
    }

    [Fact]
    [AllureFeature("GetByEmployerIdAsync")]
    [AllureStory("Positive case - get contracts by employer")]
    public async Task GetByEmployerId_ShouldReturnEmployerContracts()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var (_, employerId, artistContracts, employerContracts) = fixture.CreateMixedContracts(1, 2);
        
        foreach (var contract in artistContracts.Concat(employerContracts))
        {
            await repo.AddAsync(
                contract.ArtistId,
                contract.EmployerId,
                contract.ProjectDescription,
                contract.Status,
                contract.StartDate,
                contract.EndDate);
        }

        var results = await repo.GetAllByEmployerIdAsync(employerId);
        var enumerable = results as Contract[] ?? results.ToArray();
        enumerable.Should().HaveCount(2);
        enumerable.All(c => c.EmployerId == employerId).Should().BeTrue();
    }
}