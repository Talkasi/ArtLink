using Moq;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Services.Artwork;

namespace ArtLink.Tests.Services;

public class ArtworkServiceTests
{
    private readonly Mock<IArtworkRepository> _mockRepo;
    private readonly ArtworkService _service;
    private readonly Guid _testPortfolioId = Guid.NewGuid();
    private const string TestTitle = "Test Title";
    private const string TestDescription = "Test Description";
    private const string TestImagePath = "/images/test.jpg";

    public ArtworkServiceTests()
    {
        _mockRepo = new Mock<IArtworkRepository>();
        _service = new ArtworkService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetArtworkByIdAsync_ReturnsArtwork_WhenExists()
    {
        // Arrange
        var artworkId = Guid.NewGuid();
        var expectedArtwork = new Artwork(
            artworkId,
            _testPortfolioId,
            TestTitle,
            TestDescription,
            TestImagePath);
        
        _mockRepo.Setup(r => r.GetByIdAsync(artworkId))
            .ReturnsAsync(expectedArtwork);

        // Act
        var result = await _service.GetArtworkByIdAsync(artworkId);

        // Assert
        Assert.Equal(expectedArtwork, result);
        _mockRepo.Verify(r => r.GetByIdAsync(artworkId), Times.Once);
    }

    [Fact]
    public async Task GetAllByPortfolioIdAsync_ReturnsCorrectArtworks()
    {
        // Arrange
        var artworks = new List<Artwork>
        {
            new Artwork(Guid.NewGuid(), _testPortfolioId, "Title 1", "Desc 1", "/img1.jpg"),
            new Artwork(Guid.NewGuid(), _testPortfolioId, "Title 2", "Desc 2", "/img2.jpg")
        };
        
        _mockRepo.Setup(r => r.GetAllByPortfolioIdAsync(_testPortfolioId))
            .ReturnsAsync(artworks);

        // Act
        var result = (await _service.GetAllByPortfolioIdAsync(_testPortfolioId)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Equal(_testPortfolioId, a.PortfolioId));
        _mockRepo.Verify(r => r.GetAllByPortfolioIdAsync(_testPortfolioId), Times.Once);
    }

    [Fact]
    public async Task AddArtworkAsync_CreatesWithCorrectParameters()
    {
        // Act
        await _service.AddArtworkAsync(
            _testPortfolioId,
            TestTitle,
            TestImagePath, TestDescription);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(
            _testPortfolioId,
            TestTitle,
            TestImagePath, TestDescription), Times.Once);
    }

    [Fact]
    public async Task UpdateArtworkAsync_UpdatesCorrectArtwork()
    {
        // Arrange
        var artworkId = Guid.NewGuid();
        const string newTitle = "Updated Title";
        const string newDescription = "Updated Description";
        const string newImagePath = "/images/updated.jpg";

        // Act
        await _service.UpdateArtworkAsync(
            artworkId,
            _testPortfolioId,
            newTitle,
            newImagePath, newDescription);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(
            artworkId,
            _testPortfolioId,
            newTitle,
            newImagePath, newDescription), Times.Once);
    }

    [Fact]
    public async Task DeleteArtworkAsync_DeletesCorrectArtwork()
    {
        // Arrange
        var artworkId = Guid.NewGuid();

        // Act
        await _service.DeleteArtworkAsync(artworkId);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(artworkId), Times.Once);
    }

    [Fact]
    public async Task GetArtworkByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Artwork?)null);

        // Act
        var result = await _service.GetArtworkByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
        _mockRepo.Verify(r => r.GetByIdAsync(nonExistentId), Times.Once);
    }
}