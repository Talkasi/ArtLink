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
public class ArtworkRepositoryIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private ArtworkRepository CreateRepository()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        return new ArtworkRepository(context, new NullLogger<ArtworkRepository>());
    }

    [Fact]
    public async Task Artwork_FullCycle_WithSearchAndUpdate()
    {
        // Arrange
        var repo = CreateRepository();

        // Используем тестовые данные из init.sql
        var existingPortfolioId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Act 1 — добавить новый artwork
        var newArtworkId = await repo.AddAsync(
            existingPortfolioId,
            "New Artwork",
            "/images/new.png",
            "New description"
        );

        // Assert 1 — проверить что добавлен
        var added = await repo.GetByIdAsync(newArtworkId);
        added.Should().NotBeNull();
        added.Title.Should().Be("New Artwork");
        added.Description.Should().Be("New description");

        // Act 2 — обновить artwork
        await repo.UpdateAsync(
            newArtworkId,
            existingPortfolioId,
            "Updated Artwork",
            "/images/updated.png",
            "Updated description"
        );

        // Assert 2 — проверить обновление
        var updated = await repo.GetByIdAsync(newArtworkId);
        updated.Should().NotBeNull();
        updated.Title.Should().Be("Updated Artwork");
        updated.ImagePath.Should().Be("/images/updated.png");
        updated.Description.Should().Be("Updated description");

        // Act 3 — поиск по prompt
        var searchResults = await repo.SearchByPromptAsync("Updated");
        searchResults.Should().ContainSingle(a => a.Id == newArtworkId);

        // Act 4 — удалить
        await repo.DeleteAsync(newArtworkId);

        // Assert 4 — проверить удаление
        var deleted = await repo.GetByIdAsync(newArtworkId);
        deleted.Should().BeNull();
    }
}
