using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Services.Artist;
using Moq;

namespace ArtLink.Tests.Services;

public class ArtistServiceTests
{
    private readonly Mock<IArtistRepository> _artistRepositoryMock;
    private readonly ArtistService _artistService;

    public ArtistServiceTests()
    {
        _artistRepositoryMock = new Mock<IArtistRepository>();
        _artistService = new ArtistService(_artistRepositoryMock.Object);
    }

    [Fact]
    public async Task GetArtistByIdAsync_ReturnsArtist_WhenArtistExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new Artist(id, "hash", "artist@example.com", "Alice", "Walker", "A great artist", 5, "path/to/pic.jpg");

        _artistRepositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(expected);

        // Act
        var result = await _artistService.GetArtistByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result!.Id);
    }

    [Fact]
    public async Task GetAllArtistsAsync_ReturnsAllArtists()
    {
        // Arrange
        var artists = new List<Artist>
        {
            new Artist(Guid.NewGuid(), null, "a@example.com", "Anna", "Smith", "Bio A", 2, "picA.jpg"),
            new Artist(Guid.NewGuid(), null, "b@example.com", "Bob", "Brown", "Bio B", 3, "picB.jpg")
        };

        _artistRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(artists);

        // Act
        var result = await _artistService.GetAllArtistsAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddArtistAsync_CallsRepositoryWithCorrectParameters()
    {
        // Arrange
        const string firstName = "Emily";
        const string lastName = "Clark";
        const string email = "emily@example.com";
        const string passwordHash = "hashedPassword"; // Здесь указываем уже готовый хеш
        const string bio = "Painter from NY";
        const string profilePicturePath = "pic.jpg";
        const int experience = 4;

        // Act
        await _artistService.AddArtistAsync(firstName, lastName, email, passwordHash, bio, profilePicturePath, experience);

        // Assert
        _artistRepositoryMock.Verify(r => r.AddAsync(
            firstName, lastName, email, passwordHash, bio, profilePicturePath, experience), Times.Once);
    }

    [Fact]
    public async Task UpdateArtistAsync_CallsRepositoryWithCorrectParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string firstName = "John";
        const string lastName = "Doe";
        const string email = "john.doe@example.com";
        const string bio = "Experienced sculptor";
        const string profilePicturePath = "newpic.jpg";
        const int experience = 10;

        // Act
        await _artistService.UpdateArtistAsync(id, firstName, lastName, email, bio, profilePicturePath, experience);

        // Assert
        _artistRepositoryMock.Verify(r => r.UpdateAsync(
            id, firstName, lastName, email, bio, profilePicturePath, experience), Times.Once);
    }

    [Fact]
    public async Task DeleteArtistAsync_CallsRepositoryWithCorrectId()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        await _artistService.DeleteArtistAsync(id);

        // Assert
        _artistRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }
    
    [Fact]
    public async Task GetArtistByIdAsync_ReturnsNull_WhenArtistDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _artistRepositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Artist?)null);

        // Act
        var result = await _artistService.GetArtistByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllArtistsAsync_ReturnsEmptyList_WhenNoArtistsExist()
    {
        // Arrange
        _artistRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Artist>());

        // Act
        var result = await _artistService.GetAllArtistsAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateArtistAsync_PreservesOriginalValues_WhenPartialUpdate()
    {
        // Arrange
        var originalArtist = new Artist(
            Guid.NewGuid(), 
            "hash", 
            "original@email.com", 
            "Original", 
            "Name", 
            "Original Bio", 
            3, 
            "original.jpg"
        );

        _artistRepositoryMock.Setup(r => r.GetByIdAsync(originalArtist.Id))
            .ReturnsAsync(originalArtist);

        // Act - Обновляем только email
        await _artistService.UpdateArtistAsync(
            originalArtist.Id,
            originalArtist.FirstName,
            originalArtist.LastName,
            "new@email.com",
            originalArtist.Bio,
            originalArtist.ProfilePicturePath,
            originalArtist.Experience
        );

        // Assert
        _artistRepositoryMock.Verify(r => r.UpdateAsync(
            originalArtist.Id,
            originalArtist.FirstName,
            originalArtist.LastName,
            "new@email.com",
            originalArtist.Bio,
            originalArtist.ProfilePicturePath,
            originalArtist.Experience
        ), Times.Once);
    }
}

