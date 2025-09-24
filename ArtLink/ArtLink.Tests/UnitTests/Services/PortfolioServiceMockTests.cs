using Allure.Xunit.Attributes;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Services.Portfolio;
using ArtLink.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArtLink.Tests.UnitTests.Services;

[AllureSuite("Portfolio Service Tests")]
[AllureSubSuite("London Style (with Mocks)")]
public class PortfolioServiceMockUnitTests : IClassFixture<PortfolioFixture>
{
    private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock;
    private readonly Mock<ILogger<PortfolioService>> _loggerMock;
    private readonly PortfolioService _sut;
    private readonly PortfolioFixture _fixture;

    public PortfolioServiceMockUnitTests(PortfolioFixture fixture)
    {
        _fixture = fixture;
        _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
        _loggerMock = new Mock<ILogger<PortfolioService>>();
        _sut = new PortfolioService(_portfolioRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    [AllureFeature("GetPortfolioByIdAsync")]
    [AllureStory("Positive case - portfolio exists")]
    public async Task GetPortfolioByIdAsync_PortfolioExists_ReturnsPortfolio()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var expectedPortfolio = _fixture.CreatePortfolio(id: portfolioId);
        
        _portfolioRepositoryMock
            .Setup(repo => repo.GetByIdAsync(portfolioId))
            .ReturnsAsync(expectedPortfolio);

        // Act
        var result = await _sut.GetPortfolioByIdAsync(portfolioId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedPortfolio);
        _portfolioRepositoryMock.Verify(repo => repo.GetByIdAsync(portfolioId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetPortfolioByIdAsync")]
    [AllureStory("Negative case - portfolio not found")]
    public async Task GetPortfolioByIdAsync_PortfolioNotExists_ReturnsNull()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        
        _portfolioRepositoryMock
            .Setup(repo => repo.GetByIdAsync(portfolioId))
            .ReturnsAsync((Portfolio?)null);

        // Act
        var result = await _sut.GetPortfolioByIdAsync(portfolioId);

        // Assert
        result.Should().BeNull();
        _portfolioRepositoryMock.Verify(repo => repo.GetByIdAsync(portfolioId), Times.Once);
        
        // Verify logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Portfolio not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    [AllureFeature("GetPortfolioByIdAsync")]
    [AllureStory("Exception handling")]
    public async Task GetPortfolioByIdAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var exception = new Exception("Database error");
        
        _portfolioRepositoryMock
            .Setup(repo => repo.GetByIdAsync(portfolioId))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetPortfolioByIdAsync(portfolioId));
        _portfolioRepositoryMock.Verify(repo => repo.GetByIdAsync(portfolioId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllByArtistIdAsync")]
    [AllureStory("Positive case - portfolios exist")]
    public async Task GetAllByArtistIdAsync_PortfoliosExist_ReturnsPortfolios()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var portfolios = new List<Portfolio>
        {
            _fixture.CreatePortfolio(artistId: artistId, title: "Portfolio 1"),
            _fixture.CreatePortfolio(artistId: artistId, title: "Portfolio 2")
        };
        
        _portfolioRepositoryMock
            .Setup(repo => repo.GetAllByArtistIdAsync(artistId))
            .ReturnsAsync(portfolios);

        // Act
        var result = await _sut.GetAllByArtistIdAsync(artistId);

        // Assert
        result.Should().HaveCount(2);
        _portfolioRepositoryMock.Verify(repo => repo.GetAllByArtistIdAsync(artistId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllByArtistIdAsync")]
    [AllureStory("Negative case - no portfolios")]
    public async Task GetAllByArtistIdAsync_NoPortfolios_ReturnsEmptyList()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        
        _portfolioRepositoryMock
            .Setup(repo => repo.GetAllByArtistIdAsync(artistId))
            .ReturnsAsync(new List<Portfolio>());

        // Act
        var result = await _sut.GetAllByArtistIdAsync(artistId);

        // Assert
        result.Should().BeEmpty();
        _portfolioRepositoryMock.Verify(repo => repo.GetAllByArtistIdAsync(artistId), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllByArtistIdAsync")]
    [AllureStory("Exception handling")]
    public async Task GetAllByArtistIdAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var exception = new Exception("Database error");
        
        _portfolioRepositoryMock
            .Setup(repo => repo.GetAllByArtistIdAsync(artistId))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetAllByArtistIdAsync(artistId));
        _portfolioRepositoryMock.Verify(repo => repo.GetAllByArtistIdAsync(artistId), Times.Once);
    }

    [Fact]
    [AllureFeature("AddPortfolioAsync")]
    [AllureStory("Positive case - portfolio added")]
    public async Task AddPortfolioAsync_ValidData_ReturnsId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var artistId = Guid.NewGuid();
        var techniqueId = Guid.NewGuid();
        
        _portfolioRepositoryMock
            .Setup(repo => repo.AddAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string?>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddPortfolioAsync(artistId, "Test Portfolio", techniqueId, "Description");

        // Assert
        result.Should().Be(expectedId);
        _portfolioRepositoryMock.Verify(repo => repo.AddAsync(
            artistId, "Test Portfolio", techniqueId, "Description"), Times.Once);
    }

    [Fact]
    [AllureFeature("AddPortfolioAsync")]
    [AllureStory("Edge case - null description allowed")]
    public async Task AddPortfolioAsync_NullDescription_ReturnsId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var artistId = Guid.NewGuid();
        var techniqueId = Guid.NewGuid();
        
        _portfolioRepositoryMock
            .Setup(repo => repo.AddAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string?>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddPortfolioAsync(artistId, "Test Portfolio", techniqueId, null);

        // Assert
        result.Should().Be(expectedId);
        _portfolioRepositoryMock.Verify(repo => repo.AddAsync(
            artistId, "Test Portfolio", techniqueId, null), Times.Once);
    }

    [Fact]
    [AllureFeature("AddPortfolioAsync")]
    [AllureStory("Exception handling")]
    public async Task AddPortfolioAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var exception = new Exception("Add failed");
        
        _portfolioRepositoryMock
            .Setup(repo => repo.AddAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string?>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.AddPortfolioAsync(
            Guid.NewGuid(), "Test Portfolio", Guid.NewGuid(), "Description"));
    }

    [Fact]
    [AllureFeature("UpdatePortfolioAsync")]
    [AllureStory("Positive case - portfolio updated")]
    public async Task UpdatePortfolioAsync_ValidData_UpdatesSuccessfully()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var artistId = Guid.NewGuid();
        var techniqueId = Guid.NewGuid();
        
        _portfolioRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), 
                It.IsAny<Guid>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdatePortfolioAsync(portfolioId, artistId, "Updated Portfolio", techniqueId, "Updated Description");

        // Assert
        _portfolioRepositoryMock.Verify(repo => repo.UpdateAsync(
            portfolioId, artistId, "Updated Portfolio", techniqueId, "Updated Description"), Times.Once);
    }

    [Fact]
    [AllureFeature("UpdatePortfolioAsync")]
    [AllureStory("Exception handling")]
    public async Task UpdatePortfolioAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var exception = new Exception("Update failed");
        
        _portfolioRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), 
                It.IsAny<Guid>(), It.IsAny<string?>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.UpdatePortfolioAsync(
            Guid.NewGuid(), Guid.NewGuid(), "Test Portfolio", Guid.NewGuid(), "Description"));
    }

    [Fact]
    [AllureFeature("DeletePortfolioAsync")]
    [AllureStory("Positive case - portfolio deleted")]
    public async Task DeletePortfolioAsync_ValidId_DeletesSuccessfully()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        
        _portfolioRepositoryMock
            .Setup(repo => repo.DeleteAsync(portfolioId))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeletePortfolioAsync(portfolioId);

        // Assert
        _portfolioRepositoryMock.Verify(repo => repo.DeleteAsync(portfolioId), Times.Once);
    }

    [Fact]
    [AllureFeature("DeletePortfolioAsync")]
    [AllureStory("Exception handling")]
    public async Task DeletePortfolioAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var exception = new Exception("Delete failed");
        
        _portfolioRepositoryMock
            .Setup(repo => repo.DeleteAsync(portfolioId))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.DeletePortfolioAsync(portfolioId));
    }
}