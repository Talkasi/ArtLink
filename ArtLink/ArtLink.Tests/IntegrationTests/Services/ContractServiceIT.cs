using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Domain.Models.Enums;
using ArtLink.Services.Artwork;
using ArtLink.Services.Contract;
using ArtLink.Tests.Common;
using ArtLink.Tests.Common.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Services;

[Collection("Database collection")]
[Trait("Category", TestCategories.Integration)]
public class ContractServiceIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private ContractService CreateService()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        var repository = new ContractRepository(context, new NullLogger<ContractRepository>());
        return new ContractService(repository, new NullLogger<ContractService>());
    }

    [Fact]
    public async Task Contract_FullCycle_WithFixture()
    {
        // Arrange
        var service = CreateService();
        var existingArtistId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var existingEmployerId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        // Act 1 — добавить контракт
        var newContractId = await service.AddContractAsync(
            existingArtistId,
            existingEmployerId,
            "Integration test contract",
            ContractState.Draft,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1)
        );

        // Assert 1 — проверяем добавление
        var added = await service.GetContractByIdAsync(newContractId);
        added.Should().NotBeNull();
        added.ProjectDescription.Should().Be("Integration test contract");
        added.Status.Should().Be(ContractState.Draft);

        // Act 2 — обновить контракт
        await service.UpdateContractAsync(
            newContractId,
            existingArtistId,
            existingEmployerId,
            "Updated contract description",
            ContractState.Accepted,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(2)
        );

        // Assert 2 — проверяем обновление
        var updated = await service.GetContractByIdAsync(newContractId);
        updated.Should().NotBeNull();
        updated.ProjectDescription.Should().Be("Updated contract description");
        updated.Status.Should().Be(ContractState.Accepted);

        // Act 3 — выборка по артисту
        var artistContracts = await service.GetAllByArtistIdAsync(existingArtistId);
        artistContracts.Should().Contain(c => c.Id == newContractId);

        // Act 4 — выборка по работодателю
        var employerContracts = await service.GetAllByEmployerIdAsync(existingEmployerId);
        employerContracts.Should().Contain(c => c.Id == newContractId);

        // Act 5 — удалить
        await service.DeleteContractAsync(newContractId);

        // Assert 5 — проверяем удаление
        var deleted = await service.GetContractByIdAsync(newContractId);
        deleted.Should().BeNull();
    }
}
