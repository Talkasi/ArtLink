using Allure.Xunit.Attributes;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Services.Artwork;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArtLink.Tests.UnitTests.Services;

[AllureSuite("Artwork Service Tests")]
[AllureSubSuite("London Style (with Mocks)")]
public class ArtworkServiceMockUnitTests : IClassFixture<ArtworkFixture>
{
    private readonly Mock<IArtworkRepository> _artworkRepositoryMock;
    private readonly ArtworkService _sut;
    private readonly ArtworkFixture _fixture;

    public ArtworkServiceMockUnitTests(ArtworkFixture fixture)
    {
        _fixture = fixture;
        _artworkRepositoryMock = new Mock<IArtworkRepository>();
        var loggerMock = new Mock<ILogger<ArtworkService>>();
        _sut = new ArtworkService(_artworkRepositoryMock.Object, loggerMock.Object);
    }

    // --- GetArtworkByIdAsync ---

    [Fact]
    [AllureFeature("GetArtworkByIdAsync")]
    [AllureStory("Positive case - artwork exists")]
    public async Task GetArtworkByIdAsync_ArtworkExists_ReturnsArtwork()
    {
        // Arrange
        var artworkId = Guid.NewGuid();
        var expectedArtwork = _fixture.CreateArtwork(id: artworkId, title: "Special Artwork");

        _artworkRepositoryMock
            .Setup(r => r.GetByIdAsync(artworkId))
            .ReturnsAsync(expectedArtwork);

        // Act
        var result = await _sut.GetArtworkByIdAsync(artworkId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedArtwork);
        result.Title.Should().Be("Special Artwork");
        _artworkRepositoryMock.Verify(r => r.GetByIdAsync(artworkId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetArtworkByIdAsync")]
    [AllureStory("Negative case - artwork not found")]
    public async Task GetArtworkByIdAsync_ArtworkNotExists_ReturnsNull()
    {
        // Arrange
        var artworkId = Guid.NewGuid();

        _artworkRepositoryMock
            .Setup(r => r.GetByIdAsync(artworkId))
            .ReturnsAsync((Artwork?)null);

        // Act
        var result = await _sut.GetArtworkByIdAsync(artworkId);

        // Assert
        result.Should().BeNull();
        _artworkRepositoryMock.Verify(r => r.GetByIdAsync(artworkId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetArtworkByIdAsync")]
    [AllureStory("Exception handling")]
    public async Task GetArtworkByIdAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var artworkId = Guid.NewGuid();
        var exception = new Exception("DB error");

        _artworkRepositoryMock
            .Setup(r => r.GetByIdAsync(artworkId))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetArtworkByIdAsync(artworkId));
        _artworkRepositoryMock.Verify(r => r.GetByIdAsync(artworkId), Times.Once);
    }

    // --- GetAllByPortfolioIdAsync ---

    [Fact]
    [AllureFeature("GetAllByPortfolioIdAsync")]
    [AllureStory("Positive case - artworks exist")]
    public async Task GetAllByPortfolioIdAsync_ArtworksExist_ReturnsList()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var artworks = _fixture.CreateArtworks(3, portfolioId);

        _artworkRepositoryMock
            .Setup(r => r.GetAllByPortfolioIdAsync(portfolioId))
            .ReturnsAsync(artworks);

        // Act
        var result = await _sut.GetAllByPortfolioIdAsync(portfolioId);

        // Assert
        Artwork[] enumerable = result as Artwork[] ?? result.ToArray();
        enumerable.Should().HaveCount(3);
        enumerable.All(a => a.PortfolioId == portfolioId).Should().BeTrue();
        _artworkRepositoryMock.Verify(r => r.GetAllByPortfolioIdAsync(portfolioId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllByPortfolioIdAsync")]
    [AllureStory("Negative case - no artworks")]
    public async Task GetAllByPortfolioIdAsync_NoArtworks_ReturnsEmptyList()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();

        _artworkRepositoryMock
            .Setup(r => r.GetAllByPortfolioIdAsync(portfolioId))
            .ReturnsAsync(new List<Artwork>());

        // Act
        var result = await _sut.GetAllByPortfolioIdAsync(portfolioId);

        // Assert
        result.Should().BeEmpty();
        _artworkRepositoryMock.Verify(r => r.GetAllByPortfolioIdAsync(portfolioId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllByPortfolioIdAsync")]
    [AllureStory("Mixed portfolio IDs")]
    public async Task GetAllByPortfolioIdAsync_WithSpecificPortfolio_ReturnsOnlyMatchingArtworks()
    {
        // Arrange
        var targetPortfolioId = Guid.NewGuid();
        var otherPortfolioId = Guid.NewGuid();
        
        var targetArtworks = _fixture.CreateArtworks(2, targetPortfolioId);
        _ = _fixture.CreateArtworks(2, otherPortfolioId);

        _artworkRepositoryMock
            .Setup(r => r.GetAllByPortfolioIdAsync(targetPortfolioId))
            .ReturnsAsync(targetArtworks);

        // Act
        var result = await _sut.GetAllByPortfolioIdAsync(targetPortfolioId);

        // Assert
        var enumerable = result as Artwork[] ?? result.ToArray();
        enumerable.Should().HaveCount(2);
        enumerable.All(a => a.PortfolioId == targetPortfolioId).Should().BeTrue();
        _artworkRepositoryMock.Verify(r => r.GetAllByPortfolioIdAsync(targetPortfolioId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllByPortfolioIdAsync")]
    [AllureStory("Exception handling")]
    public async Task GetAllByPortfolioIdAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var exception = new Exception("DB error");

        _artworkRepositoryMock
            .Setup(r => r.GetAllByPortfolioIdAsync(portfolioId))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetAllByPortfolioIdAsync(portfolioId));
        _artworkRepositoryMock.Verify(r => r.GetAllByPortfolioIdAsync(portfolioId), Times.Once);
    }

    // --- AddArtworkAsync ---

    [Fact]
    [AllureFeature("AddArtworkAsync")]
    [AllureStory("Positive case - artwork added")]
    public async Task AddArtworkAsync_ValidData_ReturnsId()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();
        var testArtwork = _fixture.CreateArtwork(portfolioId: portfolioId);

        _artworkRepositoryMock
            .Setup(r => r.AddAsync(portfolioId, testArtwork.Title, testArtwork.ImagePath, testArtwork.Description))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddArtworkAsync(portfolioId, testArtwork.Title, testArtwork.ImagePath, testArtwork.Description);

        // Assert
        result.Should().Be(expectedId);
        _artworkRepositoryMock.Verify(r => r.AddAsync(portfolioId, testArtwork.Title, testArtwork.ImagePath, testArtwork.Description), Times.Once);
    }

    [Fact]
    [AllureFeature("AddArtworkAsync")]
    [AllureStory("With custom artwork data")]
    public async Task AddArtworkAsync_WithFixtureArtwork_ReturnsId()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();
        var customArtwork = _fixture.CreateArtwork(
            portfolioId: portfolioId,
            title: "Custom Title",
            description: "Custom Description",
            imagePath: "/custom/path.jpg");

        _artworkRepositoryMock
            .Setup(r => r.AddAsync(portfolioId, customArtwork.Title, customArtwork.ImagePath, customArtwork.Description))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddArtworkAsync(portfolioId, customArtwork.Title, customArtwork.ImagePath, customArtwork.Description);

        // Assert
        result.Should().Be(expectedId);
        _artworkRepositoryMock.Verify(r => r.AddAsync(portfolioId, "Custom Title", "/custom/path.jpg", "Custom Description"), Times.Once);
    }

    [Fact]
    [AllureFeature("AddArtworkAsync")]
    [AllureStory("Exception handling")]
    public async Task AddArtworkAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var testArtwork = _fixture.CreateArtwork(portfolioId: portfolioId);
        var exception = new Exception("Add failed");

        _artworkRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.AddArtworkAsync(portfolioId, testArtwork.Title, testArtwork.ImagePath, testArtwork.Description));
        _artworkRepositoryMock.Verify(r => r.AddAsync(portfolioId, testArtwork.Title, testArtwork.ImagePath, testArtwork.Description), Times.Once);
    }

    // --- UpdateArtworkAsync ---

    [Fact]
    [AllureFeature("UpdateArtworkAsync")]
    [AllureStory("Positive case - artwork updated")]
    public async Task UpdateArtworkAsync_ValidData_UpdatesSuccessfully()
    {
        // Arrange
        var artworkId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();
        var updatedArtwork = _fixture.CreateArtwork(
            id: artworkId,
            portfolioId: portfolioId,
            title: "Updated Title",
            description: "Updated Description");

        _artworkRepositoryMock
            .Setup(r => r.UpdateAsync(artworkId, portfolioId, updatedArtwork.Title, updatedArtwork.ImagePath, updatedArtwork.Description))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateArtworkAsync(artworkId, portfolioId, updatedArtwork.Title, updatedArtwork.ImagePath, updatedArtwork.Description);

        // Assert
        _artworkRepositoryMock.Verify(r => r.UpdateAsync(artworkId, portfolioId, "Updated Title", updatedArtwork.ImagePath, "Updated Description"), Times.Once);
    }
    
    [Fact]
    [AllureFeature("UpdateArtworkAsync")]
    [AllureStory("Exception handling")]
    public async Task UpdateArtworkAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var artworkId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();
        var testArtwork = _fixture.CreateArtwork(id: artworkId, portfolioId: portfolioId);
        var exception = new Exception("Update failed");

        _artworkRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.UpdateArtworkAsync(artworkId, portfolioId, testArtwork.Title, testArtwork.ImagePath, testArtwork.Description));
        _artworkRepositoryMock.Verify(r => r.UpdateAsync(artworkId, portfolioId, testArtwork.Title, testArtwork.ImagePath, testArtwork.Description), Times.Once);
    }

    // --- DeleteArtworkAsync ---

    [Fact]
    [AllureFeature("DeleteArtworkAsync")]
    [AllureStory("Positive case - artwork deleted")]
    public async Task DeleteArtworkAsync_ValidId_DeletesSuccessfully()
    {
        // Arrange
        var artworkId = Guid.NewGuid();

        _artworkRepositoryMock
            .Setup(r => r.DeleteAsync(artworkId))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteArtworkAsync(artworkId);

        // Assert
        _artworkRepositoryMock.Verify(r => r.DeleteAsync(artworkId), Times.Once);
    }

    [Fact]
    [AllureFeature("DeleteArtworkAsync")]
    [AllureStory("With existing artwork")]
    public async Task DeleteArtworkAsync_WithFixtureArtwork_DeletesSuccessfully()
    {
        // Arrange
        var existingArtwork = _fixture.CreateArtwork();
        
        _artworkRepositoryMock
            .Setup(r => r.DeleteAsync(existingArtwork.Id))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteArtworkAsync(existingArtwork.Id);

        // Assert
        _artworkRepositoryMock.Verify(r => r.DeleteAsync(existingArtwork.Id), Times.Once);
    }

    [Fact]
    [AllureFeature("DeleteArtworkAsync")]
    [AllureStory("Exception handling")]
    public async Task DeleteArtworkAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var artworkId = Guid.NewGuid();
        var exception = new Exception("Delete failed");

        _artworkRepositoryMock
            .Setup(r => r.DeleteAsync(artworkId))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.DeleteArtworkAsync(artworkId));
        _artworkRepositoryMock.Verify(r => r.DeleteAsync(artworkId), Times.Once);
    }
}