using ArtLink.Services.Portfolio;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using Moq;

namespace ArtLink.Tests.Services;

public class PortfolioServiceTests
{
    private readonly Mock<IPortfolioRepository> _portfolioRepoMock;
    private readonly PortfolioService _portfolioService;

    public PortfolioServiceTests()
    {
        _portfolioRepoMock = new Mock<IPortfolioRepository>();
        _portfolioService = new PortfolioService(_portfolioRepoMock.Object);
    }

    [Fact]
    public async Task GetPortfolioByIdAsync_ReturnsPortfolio_WhenFound()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var expectedPortfolio = new Portfolio(portfolioId, Guid.NewGuid(), Guid.NewGuid(), "Test", "Desc");

        _portfolioRepoMock.Setup(r => r.GetByIdAsync(portfolioId))
            .ReturnsAsync(expectedPortfolio);

        // Act
        var result = await _portfolioService.GetPortfolioByIdAsync(portfolioId);

        // Assert
        Assert.Equal(expectedPortfolio, result);
    }

    [Fact]
    public async Task GetAllByArtistIdAsync_ReturnsList()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var portfolios = new List<Portfolio>
        {
            new Portfolio(Guid.NewGuid(), artistId, Guid.NewGuid(), "Title1", "Desc1"),
            new Portfolio(Guid.NewGuid(), artistId, Guid.NewGuid(), "Title2", "Desc2")
        };

        _portfolioRepoMock.Setup(r => r.GetAllByArtistIdAsync(artistId))
            .ReturnsAsync(portfolios);

        // Act
        var result = await _portfolioService.GetAllByArtistIdAsync(artistId);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddPortfolioAsync_CallsRepository()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var techniqueId = Guid.NewGuid();
        const string title = "New Portfolio";
        const string description = "Desc";

        // Act
        await _portfolioService.AddPortfolioAsync(artistId, title, techniqueId, description);

        // Assert
        _portfolioRepoMock.Verify(r =>
            r.AddAsync(artistId, title, techniqueId, description), Times.Once);
    }

    [Fact]
    public async Task UpdatePortfolioAsync_CallsRepository()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        var artistId = Guid.NewGuid();
        var techniqueId = Guid.NewGuid();
        const string title = "Updated";
        const string description = "Updated desc";

        // Act
        await _portfolioService.UpdatePortfolioAsync(portfolioId, artistId, title, techniqueId, description);

        // Assert
        _portfolioRepoMock.Verify(r =>
            r.UpdateAsync(portfolioId, artistId, title, techniqueId, description), Times.Once);
    }


    [Fact]
    public async Task DeletePortfolioAsync_CallsRepository()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();

        // Act
        await _portfolioService.DeletePortfolioAsync(portfolioId);

        // Assert
        _portfolioRepoMock.Verify(r => r.DeleteAsync(portfolioId), Times.Once);
    }
}
