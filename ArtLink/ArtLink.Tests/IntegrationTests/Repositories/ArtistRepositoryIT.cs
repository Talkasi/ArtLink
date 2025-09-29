using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Tests.Common;
using ArtLink.Tests.Common.Database;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Repositories;

[Collection("Database collection")]
[Trait("Category", TestCategories.Integration)]
public class ArtistRepositoryIt(DatabaseFixture db, ArtistFixture artistFixture) : IClassFixture<DatabaseFixture>, IClassFixture<ArtistFixture>
{
    private ArtistRepository CreateRepository()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        return new ArtistRepository(context, new NullLogger<ArtistRepository>());
    }

    [Fact]
    public async Task Artist_FullCycle_WithSearchAndUpdate()
    {
        // Arrange
        var repo = CreateRepository();

        var artist1 = artistFixture.CreateArtist(email: "artist1@test.com", firstName: "Alice");
        var artist2 = artistFixture.CreateArtist(email: "artist2@test.com", firstName: "Bob");

        // Act 1 — добавить двух артистов
        var id1 = await repo.AddAsync(
            artist1.FirstName, artist1.LastName, artist1.Email, artist1.PasswordHash!,
            artist1.Bio, artist1.ProfilePicturePath, artist1.Experience);

        var id2 = await repo.AddAsync(
            artist2.FirstName, artist2.LastName, artist2.Email, artist2.PasswordHash!,
            artist2.Bio, artist2.ProfilePicturePath, artist2.Experience);

        // Assert 1 — проверка добавления
        var fetched1 = await repo.GetByIdAsync(id1);
        var fetched2 = await repo.GetByIdAsync(id2);
        fetched1.Should().NotBeNull();
        fetched2.Should().NotBeNull();

        // Act 2 — обновление
        fetched1.FirstName = "AliceUpdated";
        fetched1.LastName = "Smith";
        await repo.UpdateAsync(id1, fetched1.FirstName, fetched1.LastName, fetched1.Email,
                               fetched1.Bio, fetched1.ProfilePicturePath, fetched1.Experience);

        // Assert 2 — проверка обновления
        var updated1 = await repo.GetByIdAsync(id1);
        updated1!.FirstName.Should().Be("AliceUpdated");
        updated1.LastName.Should().Be("Smith");

        // Act 3 — поиск по prompt
        var searchResults = await repo.SearchByPromptAsync("AliceUpdated");
        searchResults.Should().ContainSingle(a => a.Id == id1);

        var searchResults2 = await repo.SearchByPromptAsync("Bob");
        searchResults2.Should().ContainSingle(a => a.Id == id2);

        // Act 4 — удалить
        await repo.DeleteAsync(id1);
        await repo.DeleteAsync(id2);

        // Assert 4 — проверка удаления
        var deleted1 = await repo.GetByIdAsync(id1);
        var deleted2 = await repo.GetByIdAsync(id2);
        deleted1.Should().BeNull();
        deleted2.Should().BeNull();
    }
}

