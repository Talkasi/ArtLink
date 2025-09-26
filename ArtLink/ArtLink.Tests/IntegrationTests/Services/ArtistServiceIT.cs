using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Services.Artist;
using ArtLink.Tests.Common.Database;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Services;

[Collection("Database collection")]
public class ArtistServiceIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private readonly ArtistFixture _artistFixture = new ArtistFixture();

    private ArtistService CreateService()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        var repository = new ArtistRepository(context, new NullLogger<ArtistRepository>());
        return new ArtistService(repository, new NullLogger<ArtistService>());
    }

    [Fact]
    public async Task Artist_FullCycle_WithLogin_UsingFixture()
    {
        var service = CreateService();

        // Используем фикстуру для генерации артиста
        var testArtist = _artistFixture.CreateArtist();

        // Act 1 — добавить артиста
        var artistId = await service.AddArtistAsync(
            testArtist.FirstName,
            testArtist.LastName,
            testArtist.Email,
            testArtist.PasswordHash!,
            testArtist.Bio,
            testArtist.ProfilePicturePath,
            testArtist.Experience
        );

        // Assert 1 — получить по ID
        var addedArtist = await service.GetArtistByIdAsync(artistId);
        addedArtist.Should().NotBeNull();
        addedArtist.Email.Should().Be(testArtist.Email);

        // Act 2 — обновить имя
        var updatedFirstName = "UpdatedName";
        await service.UpdateArtistAsync(
            artistId,
            updatedFirstName,
            testArtist.LastName,
            testArtist.Email,
            testArtist.Bio,
            testArtist.ProfilePicturePath,
            testArtist.Experience
        );

        var updatedArtist = await service.GetArtistByIdAsync(artistId);
        updatedArtist.Should().NotBeNull();
        updatedArtist.FirstName.Should().Be(updatedFirstName);

        // Act 3 — логин
        var loginResult = await service.LoginArtistAsync(testArtist.Email, testArtist.PasswordHash!);
        loginResult.Should().NotBeNull();
        loginResult.Id.Should().Be(artistId);

        // Act 4 — удалить
        await service.DeleteArtistAsync(artistId);

        // Assert 4 — проверка удаления
        var deletedArtist = await service.GetArtistByIdAsync(artistId);
        deletedArtist.Should().BeNull();
    }
}