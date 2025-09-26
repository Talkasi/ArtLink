using Allure.Xunit.Attributes;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Services.Artist;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArtLink.Tests.UnitTests.Services;

[AllureSuite("Artist Service Tests")]
[AllureSubSuite("London Style (with Mocks)")]
public class ArtistServiceMockUnitTests : IClassFixture<ArtistFixture>
{
    private readonly Mock<IArtistRepository> _artistRepositoryMock;
    private readonly ArtistService _sut;
    private readonly ArtistFixture _fixture;

    public ArtistServiceMockUnitTests(ArtistFixture fixture)
    {
        _fixture = fixture;
        _artistRepositoryMock = new Mock<IArtistRepository>();
        Mock<ILogger<ArtistService>> loggerMock = new Mock<ILogger<ArtistService>>();
        _sut = new ArtistService(_artistRepositoryMock.Object, loggerMock.Object);
    }
    
    [Fact]
    [AllureFeature("GetArtistByIdAsync")]
    [AllureStory("Positive case - artist exists")]
    public async Task GetArtistByIdAsync_ArtistExists_ReturnsArtist()
    {
        var artistId = Guid.NewGuid();
        var expectedArtist = _fixture.CreateArtist(id: artistId);
        
        _artistRepositoryMock
            .Setup(repo => repo.GetByIdAsync(artistId))
            .ReturnsAsync(expectedArtist);

        // Act
        var result = await _sut.GetArtistByIdAsync(artistId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedArtist);
        _artistRepositoryMock.Verify(repo => repo.GetByIdAsync(artistId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetArtistByIdAsync")]
    [AllureStory("Negative case - artist not found")]
    public async Task GetArtistByIdAsync_ArtistNotExists_ReturnsNull()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        
        _artistRepositoryMock
            .Setup(repo => repo.GetByIdAsync(artistId))
            .ReturnsAsync((Artist?)null);

        // Act
        var result = await _sut.GetArtistByIdAsync(artistId);

        // Assert
        result.Should().BeNull();
        _artistRepositoryMock.Verify(repo => repo.GetByIdAsync(artistId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetArtistByIdAsync")]
    [AllureStory("Exception handling")]
    public async Task GetArtistByIdAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var exception = new Exception("Database error");
        
        _artistRepositoryMock
            .Setup(repo => repo.GetByIdAsync(artistId))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetArtistByIdAsync(artistId));
        _artistRepositoryMock.Verify(repo => repo.GetByIdAsync(artistId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllArtistsAsync")]
    [AllureStory("Positive case - artists exist")]
    public async Task GetAllArtistsAsync_ArtistsExist_ReturnsArtists()
    {
        // Arrange
        var artists = new List<Artist>
        {
            _fixture.CreateArtist(email: "artist1@example.com"),
            _fixture.CreateArtist(email: "artist2@example.com")
        };
        
        _artistRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(artists);

        // Act
        var result = await _sut.GetAllArtistsAsync();

        // Assert
        result.Should().HaveCount(2);
        _artistRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllArtistsAsync")]
    [AllureStory("Negative case - no artists")]
    public async Task GetAllArtistsAsync_NoArtists_ReturnsEmptyList()
    {
        // Arrange
        _artistRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Artist>());

        // Act
        var result = await _sut.GetAllArtistsAsync();

        // Assert
        result.Should().BeEmpty();
        _artistRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllArtistsAsync")]
    [AllureStory("Exception handling")]
    public async Task GetAllArtistsAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var exception = new Exception("Database error");
        
        _artistRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetAllArtistsAsync());
        _artistRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    [AllureFeature("AddArtistAsync")]
    [AllureStory("Positive case - artist added")]
    public async Task AddArtistAsync_ValidData_ReturnsId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        
        _artistRepositoryMock
            .Setup(repo => repo.AddAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int?>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddArtistAsync(
            "John", "Doe", "john@example.com", "hash123",
            "Bio", "/profile.jpg", 5);

        // Assert
        result.Should().Be(expectedId);
        _artistRepositoryMock.Verify(repo => repo.AddAsync(
            "John", "Doe", "john@example.com", "hash123",
            "Bio", "/profile.jpg", 5), Times.Once);
    }

    [Fact]
    [AllureFeature("AddArtistAsync")]
    [AllureStory("Exception handling")]
    public async Task AddArtistAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var exception = new Exception("Add failed");
        
        _artistRepositoryMock
            .Setup(repo => repo.AddAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int?>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.AddArtistAsync(
            "John", "Doe", "john@example.com", "hash123",
            "Bio", "/profile.jpg", 5));
    }

    [Fact]
    [AllureFeature("UpdateArtistAsync")]
    [AllureStory("Positive case - artist updated")]
    public async Task UpdateArtistAsync_ValidData_UpdatesSuccessfully()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var artist = _fixture.CreateArtist(id: artistId);
        
        _artistRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int?>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateArtistAsync(
            artistId, artist.FirstName, artist.LastName, artist.Email,
            artist.Bio, artist.ProfilePicturePath, artist.Experience);

        // Assert
        _artistRepositoryMock.Verify(repo => repo.UpdateAsync(
            artistId, artist.FirstName, artist.LastName, artist.Email,
            artist.Bio, artist.ProfilePicturePath, artist.Experience), Times.Once);
    }

    [Fact]
    [AllureFeature("UpdateArtistAsync")]
    [AllureStory("Exception handling")]
    public async Task UpdateArtistAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var exception = new Exception("Update failed");
        
        _artistRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int?>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.UpdateArtistAsync(
            artistId, "John", "Doe", "john@example.com",
            "Bio", "/profile.jpg", 5));
    }

    [Fact]
    [AllureFeature("DeleteArtistAsync")]
    [AllureStory("Positive case - artist deleted")]
    public async Task DeleteArtistAsync_ValidId_DeletesSuccessfully()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        
        _artistRepositoryMock
            .Setup(repo => repo.DeleteAsync(artistId))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteArtistAsync(artistId);

        // Assert
        _artistRepositoryMock.Verify(repo => repo.DeleteAsync(artistId), Times.Once);
    }

    [Fact]
    [AllureFeature("DeleteArtistAsync")]
    [AllureStory("Exception handling")]
    public async Task DeleteArtistAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var exception = new Exception("Delete failed");
        
        _artistRepositoryMock
            .Setup(repo => repo.DeleteAsync(artistId))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.DeleteArtistAsync(artistId));
    }

    [Fact]
    [AllureFeature("LoginArtistAsync")]
    [AllureStory("Positive case - valid credentials")]
    public async Task LoginArtistAsync_ValidCredentials_ReturnsArtist()
    {
        // Arrange
        var expectedArtist = _fixture.CreateArtist();
        
        _artistRepositoryMock
            .Setup(repo => repo.LoginAsync("test@example.com", "correct_hash"))
            .ReturnsAsync(expectedArtist);

        // Act
        var result = await _sut.LoginArtistAsync("test@example.com", "correct_hash");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedArtist);
        _artistRepositoryMock.Verify(repo => repo.LoginAsync("test@example.com", "correct_hash"), Times.Once);
    }

    [Fact]
    [AllureFeature("LoginArtistAsync")]
    [AllureStory("Negative case - invalid credentials")]
    public async Task LoginArtistAsync_InvalidCredentials_ReturnsNull()
    {
        // Arrange
        _artistRepositoryMock
            .Setup(repo => repo.LoginAsync("wrong@example.com", "wrong_hash"))
            .ReturnsAsync((Artist?)null);

        // Act
        var result = await _sut.LoginArtistAsync("wrong@example.com", "wrong_hash");

        // Assert
        result.Should().BeNull();
        _artistRepositoryMock.Verify(repo => repo.LoginAsync("wrong@example.com", "wrong_hash"), Times.Once);
    }

    [Fact]
    [AllureFeature("LoginArtistAsync")]
    [AllureStory("Exception handling")]
    public async Task LoginArtistAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var exception = new Exception("Login failed");
        
        _artistRepositoryMock
            .Setup(repo => repo.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.LoginArtistAsync("test@example.com", "hash"));
    }
}