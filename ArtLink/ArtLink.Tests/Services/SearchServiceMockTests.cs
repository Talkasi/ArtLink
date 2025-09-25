using Allure.Xunit.Attributes;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Services.Search;
using ArtLink.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArtLink.Tests.Services;

[AllureSuite("Search Service Tests")]
[AllureSubSuite("London Style (with Mocks)")]
public class SearchServiceMockTests : IClassFixture<ArtistFixture>, IClassFixture<EmployerFixture>, IClassFixture<ArtworkFixture>
{
    private readonly Mock<IArtistRepository> _artistRepoMock;
    private readonly Mock<IEmployerRepository> _employerRepoMock;
    private readonly Mock<IArtworkRepository> _artworkRepoMock;
    private readonly SearchService _sut;
    private readonly ArtistFixture _artistFixture;
    private readonly EmployerFixture _employerFixture;
    private readonly ArtworkFixture _artworkFixture;

    public SearchServiceMockTests(ArtistFixture artistFixture, EmployerFixture employerFixture, ArtworkFixture artworkFixture)
    {
        _artistFixture = artistFixture;
        _employerFixture = employerFixture;
        _artworkFixture = artworkFixture;
        
        _artistRepoMock = new Mock<IArtistRepository>();
        _employerRepoMock = new Mock<IEmployerRepository>();
        _artworkRepoMock = new Mock<IArtworkRepository>();
        Mock<ILogger<SearchService>> loggerMock = new Mock<ILogger<SearchService>>();
        _sut = new SearchService(
            _artistRepoMock.Object,
            _employerRepoMock.Object,
            _artworkRepoMock.Object,
            loggerMock.Object);
    }

    [Fact]
    [AllureFeature("SearchArtistsByPromptAsync")]
    [AllureStory("Positive - returns matching artists")]
    public async Task SearchArtistsByPromptAsync_ReturnsArtists_WhenMatchesExist()
    {
        // Arrange
        var prompt = "portrait";
        var list = new List<Artist> { _artistFixture.CreateArtist(), _artistFixture.CreateArtist(email: "a2@example.com") };
        _artistRepoMock.Setup(r => r.SearchByPromptAsync(prompt)).ReturnsAsync(list);

        // Act
        var result = (await _sut.SearchArtistsByPromptAsync(prompt)).ToList();

        // Assert
        result.Should().HaveCount(2);
        _artistRepoMock.Verify(r => r.SearchByPromptAsync(prompt), Times.Once);
    }

    [Fact]
    [AllureFeature("SearchArtistsByPromptAsync")]
    [AllureStory("Negative - returns empty when no matches")]
    public async Task SearchArtistsByPromptAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var prompt = "nonexisting";
        _artistRepoMock.Setup(r => r.SearchByPromptAsync(prompt)).ReturnsAsync(new List<Artist>());

        // Act
        var result = await _sut.SearchArtistsByPromptAsync(prompt);

        // Assert
        result.Should().BeEmpty();
        _artistRepoMock.Verify(r => r.SearchByPromptAsync(prompt), Times.Once);
    }

    [Fact]
    [AllureFeature("SearchArtistsByPromptAsync")]
    [AllureStory("Exception handling - repository throws")]
    public async Task SearchArtistsByPromptAsync_Throws_WhenRepositoryThrows()
    {
        // Arrange
        var prompt = "err";
        _artistRepoMock.Setup(r => r.SearchByPromptAsync(prompt)).ThrowsAsync(new Exception("DB fail"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.SearchArtistsByPromptAsync(prompt));
    }

    // ---------------------------
    // SearchEmployersByPromptAsync
    // ---------------------------

    [Fact]
    [AllureFeature("SearchEmployersByPromptAsync")]
    [AllureStory("Positive - returns matching employers")]
    public async Task SearchEmployersByPromptAsync_ReturnsEmployers_WhenMatchesExist()
    {
        // Arrange
        var prompt = "design";
        var list = new List<Employer> { _employerFixture.CreateEmployer(), _employerFixture.CreateEmployer(companyName: "SecondCo", email: "b@e.com") };
        _employerRepoMock.Setup(r => r.SearchByPromptAsync(prompt)).ReturnsAsync(list);

        // Act
        var result = (await _sut.SearchEmployersByPromptAsync(prompt)).ToList();

        // Assert
        result.Should().HaveCount(2);
        _employerRepoMock.Verify(r => r.SearchByPromptAsync(prompt), Times.Once);
    }

    [Fact]
    [AllureFeature("SearchEmployersByPromptAsync")]
    [AllureStory("Negative - returns empty when no matches")]
    public async Task SearchEmployersByPromptAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var prompt = "zzz";
        _employerRepoMock.Setup(r => r.SearchByPromptAsync(prompt)).ReturnsAsync(new List<Employer>());

        // Act
        var result = await _sut.SearchEmployersByPromptAsync(prompt);

        // Assert
        result.Should().BeEmpty();
        _employerRepoMock.Verify(r => r.SearchByPromptAsync(prompt), Times.Once);
    }

    [Fact]
    [AllureFeature("SearchEmployersByPromptAsync")]
    [AllureStory("Exception handling - repository throws")]
    public async Task SearchEmployersByPromptAsync_Throws_WhenRepositoryThrows()
    {
        // Arrange
        var prompt = "err";
        _employerRepoMock.Setup(r => r.SearchByPromptAsync(prompt)).ThrowsAsync(new Exception("DB fail"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.SearchEmployersByPromptAsync(prompt));
    }

    // ---------------------------
    // SearchArtWorksByPromptAsync
    // ---------------------------

    [Fact]
    [AllureFeature("SearchArtWorksByPromptAsync")]
    [AllureStory("Positive - returns matching artworks")]
    public async Task SearchArtWorksByPromptAsync_ReturnsArtworks_WhenMatchesExist()
    {
        // Arrange
        var prompt = "landscape";
        var list = new List<Artwork> { _artworkFixture.CreateArtwork(title: "A1"), _artworkFixture.CreateArtwork(title: "A2") };
        _artworkRepoMock.Setup(r => r.SearchByPromptAsync(prompt)).ReturnsAsync(list);

        // Act
        var result = (await _sut.SearchArtWorksByPromptAsync(prompt)).ToList();

        // Assert
        result.Should().HaveCount(2);
        _artworkRepoMock.Verify(r => r.SearchByPromptAsync(prompt), Times.Once);
    }

    [Fact]
    [AllureFeature("SearchArtWorksByPromptAsync")]
    [AllureStory("Negative - returns empty when no matches")]
    public async Task SearchArtWorksByPromptAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var prompt = "nothing";
        _artworkRepoMock.Setup(r => r.SearchByPromptAsync(prompt)).ReturnsAsync(new List<Artwork>());

        // Act
        var result = await _sut.SearchArtWorksByPromptAsync(prompt);

        // Assert
        result.Should().BeEmpty();
        _artworkRepoMock.Verify(r => r.SearchByPromptAsync(prompt), Times.Once);
    }

    [Fact]
    [AllureFeature("SearchArtWorksByPromptAsync")]
    [AllureStory("Exception handling - repository throws")]
    public async Task SearchArtWorksByPromptAsync_Throws_WhenRepositoryThrows()
    {
        // Arrange
        var prompt = "err";
        _artworkRepoMock.Setup(r => r.SearchByPromptAsync(prompt)).ThrowsAsync(new Exception("DB fail"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.SearchArtWorksByPromptAsync(prompt));
    }
}
