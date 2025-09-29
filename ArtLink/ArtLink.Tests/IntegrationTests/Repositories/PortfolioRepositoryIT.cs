using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Tests.Common;
using ArtLink.Tests.Common.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Repositories;

[Collection("Database collection")]
[Trait("Category", TestCategories.Integration)]
public class PortfolioRepositoryIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private PortfolioRepository CreateRepository()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        return new PortfolioRepository(context, new NullLogger<PortfolioRepository>());
    }

    [Fact]
    public async Task Portfolio_FullCycle()
    {
        // Arrange
        var repo = CreateRepository();

        var existingArtistId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var existingTechniqueId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Act 1 — добавить портфолио
        var newPortfolioId = await repo.AddAsync(
            existingArtistId,
            "Test Portfolio",
            existingTechniqueId,
            "Portfolio description"
        );

        // Assert 1 — проверить что добавлено
        var added = await repo.GetByIdAsync(newPortfolioId);
        added.Should().NotBeNull();
        added.Title.Should().Be("Test Portfolio");
        added.Description.Should().Be("Portfolio description");

        // Act 2 — обновить портфолио
        await repo.UpdateAsync(
            newPortfolioId,
            existingArtistId,
            "Updated Portfolio",
            existingTechniqueId,
            "Updated description"
        );

        // Assert 2 — проверить обновление
        var updated = await repo.GetByIdAsync(newPortfolioId);
        updated.Should().NotBeNull();
        updated.Title.Should().Be("Updated Portfolio");
        updated.Description.Should().Be("Updated description");

        // Act 3 — выборка по артисту
        var artistPortfolios = await repo.GetAllByArtistIdAsync(existingArtistId);
        artistPortfolios.Should().Contain(p => p.Id == newPortfolioId);

        // Act 4 — удалить портфолио
        await repo.DeleteAsync(newPortfolioId);

        // Assert 4 — проверить удаление
        var deleted = await repo.GetByIdAsync(newPortfolioId);
        deleted.Should().BeNull();
    }
}
