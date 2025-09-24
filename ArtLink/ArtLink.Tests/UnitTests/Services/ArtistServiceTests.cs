using Allure.Xunit.Attributes;
using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Services.Artist;
using ArtLink.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.UnitTests.Services;

[AllureSuite("Artist Service Tests")]
[AllureSubSuite("Classical Style (SQLite InMemory)")]
public class ArtistServiceUnitTests : IDisposable, IClassFixture<ArtistFixture>
{
    private readonly SqliteConnection _connection;
    private readonly ArtLinkDbContext _dbContext;
    private readonly ArtistService _sut;
    private readonly ArtistFixture _fixture;

    public ArtistServiceUnitTests(ArtistFixture fixture)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new ArtLinkDbContext(options);
        _dbContext.Database.EnsureCreated();
        
        _fixture = fixture;

        var artistRepository = new ArtistRepository(_dbContext,  NullLogger<ArtistRepository>.Instance);
        _sut = new ArtistService(artistRepository, NullLogger<ArtistService>.Instance);
    }

    [Fact]
    [AllureFeature("AddArtistAsync + GetArtistByIdAsync")]
    [AllureStory("Positive case - add and fetch")]
    public async Task AddAndGetArtist_ShouldReturnSameArtist()
    {
        var artist = _fixture.CreateArtist(); 
        // Arrange
        var id = await _sut.AddArtistAsync(artist.FirstName, artist.LastName, artist.Email, artist.PasswordHash!,
                                           artist.Bio, artist.ProfilePicturePath, artist.Experience);

        // Act
        var result = await _sut.GetArtistByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(artist.Email);
    }

    [Fact]
    [AllureFeature("GetAllArtistsAsync")]
    [AllureStory("Positive case - multiple artists")]
    public async Task GetAllArtists_ShouldReturnAll()
    {
        var artist1 = _fixture.CreateArtist(); 
        var artist2 = _fixture.CreateArtist(); 
        // Arrange
        await _sut.AddArtistAsync(artist1.FirstName, artist1.LastName, artist1.Email, artist1.PasswordHash!,
            artist1.Bio, artist1.ProfilePicturePath, artist1.Experience);
        await _sut.AddArtistAsync(artist2.FirstName, artist2.LastName, artist2.Email, artist2.PasswordHash!,
            artist2.Bio, artist2.ProfilePicturePath, artist2.Experience);

        // Act
        var result = await _sut.GetAllArtistsAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    [AllureFeature("UpdateArtistAsync")]
    [AllureStory("Positive case - artist updated")]
    public async Task UpdateArtist_ShouldChangeFields()
    {
        var artist = _fixture.CreateArtist(); 
        // Arrange
        var id = await _sut.AddArtistAsync(artist.FirstName, artist.LastName, artist.Email, artist.PasswordHash!,
            artist.Bio, artist.ProfilePicturePath, artist.Experience);

        // Act
        await _sut.UpdateArtistAsync(id, "Johnny", artist.LastName, artist.Email,
            artist.Bio, artist.ProfilePicturePath, 10);

        var updated = await _sut.GetArtistByIdAsync(id);

        // Assert
        updated.Should().NotBeNull();
        updated.FirstName.Should().Be("Johnny");
        updated.Experience.Should().Be(10);
    }

    [Fact]
    [AllureFeature("DeleteArtistAsync")]
    [AllureStory("Positive case - artist deleted")]
    public async Task DeleteArtist_ShouldRemoveFromDb()
    {
        // Arrange
        var artist = _fixture.CreateArtist(); 
        var id = await _sut.AddArtistAsync(artist.FirstName, artist.LastName, artist.Email, artist.PasswordHash!,
            artist.Bio, artist.ProfilePicturePath, artist.Experience);

        // Act
        await _sut.DeleteArtistAsync(id);
        var deleted = await _sut.GetArtistByIdAsync(id);

        // Assert
        deleted.Should().BeNull();
    }

    [Fact]
    [AllureFeature("LoginArtistAsync")]
    [AllureStory("Positive case - valid credentials")]
    public async Task LoginArtist_ShouldReturnArtist_WhenCredentialsMatch()
    {
        // Arrange
        var artist = _fixture.CreateArtist();
        await _sut.AddArtistAsync(artist.FirstName, artist.LastName, artist.Email, artist.PasswordHash!,
            artist.Bio, artist.ProfilePicturePath, artist.Experience);

        // Act
        var loggedIn = await _sut.LoginArtistAsync(artist.Email, artist.PasswordHash!);

        // Assert
        loggedIn.Should().NotBeNull();
        loggedIn.Email.Should().Be(artist.Email);
    }

    [Fact]
    [AllureFeature("LoginArtistAsync")]
    [AllureStory("Negative case - wrong password")]
    public async Task LoginArtist_ShouldReturnNull_WhenWrongPassword()
    {
        // Arrange
        var artist = _fixture.CreateArtist();
        await _sut.AddArtistAsync(artist.FirstName, artist.LastName, artist.Email, artist.PasswordHash!,
            artist.Bio, artist.ProfilePicturePath, artist.Experience);

        // Act
        var loggedIn = await _sut.LoginArtistAsync(artist.Email, "wrong");

        // Assert
        loggedIn.Should().BeNull();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}
