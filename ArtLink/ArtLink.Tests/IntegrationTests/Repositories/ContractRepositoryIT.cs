using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Domain.Models.Enums;
using ArtLink.Tests.Common.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Repositories;

[Collection("Database collection")]
public class ContractRepositoryIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private ContractRepository CreateRepository()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        return new ContractRepository(context, new NullLogger<ContractRepository>());
    }

    [Fact]
    public async Task Contract_FullCycle()
    {
        // Arrange
        var repo = CreateRepository();

        var existingArtistId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var existingEmployerId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        // Act 1 — добавить контракт
        var newContractId = await repo.AddAsync(
            existingArtistId,
            existingEmployerId,
            "Test integration contract",
            ContractState.Draft,
            DateTime.Today.ToUniversalTime(),
            DateTime.Today.AddMonths(1).ToUniversalTime()
        );

        // Assert 1 — проверить что добавлен
        var added = await repo.GetByIdAsync(newContractId);
        added.Should().NotBeNull();
        added.ProjectDescription.Should().Be("Test integration contract");
        added.Status.Should().Be(ContractState.Draft);

        // Act 2 — обновить контракт
        await repo.UpdateAsync(
            newContractId,
            existingArtistId,
            existingEmployerId,
            "Updated project",
            ContractState.Accepted,
            DateTime.Today.ToUniversalTime(),
            DateTime.Today.AddMonths(2).ToUniversalTime()
        );

        // Assert 2 — проверить обновление
        var updated = await repo.GetByIdAsync(newContractId);
        updated.Should().NotBeNull();
        updated.ProjectDescription.Should().Be("Updated project");
        updated.Status.Should().Be(ContractState.Accepted);

        // Act 3 — выборка по артисту
        var artistContracts = await repo.GetAllByArtistIdAsync(existingArtistId);
        artistContracts.Should().Contain(c => c.Id == newContractId);

        // Act 4 — выборка по работодателю
        var employerContracts = await repo.GetAllByEmployerIdAsync(existingEmployerId);
        employerContracts.Should().Contain(c => c.Id == newContractId);

        // Act 5 — удалить
        await repo.DeleteAsync(newContractId);

        // Assert 5 — проверить удаление
        var deleted = await repo.GetByIdAsync(newContractId);
        deleted.Should().BeNull();
    }
}
