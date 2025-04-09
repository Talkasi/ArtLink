using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Models;
using Moq;

namespace ArtLink.Tests;

public class SearchServiceTests
{
    private readonly Mock<ISearchService> _searchServiceMock = new Mock<ISearchService>();

    [Fact]
    public async Task SearchArtistsByPromptAsync_ReturnsMatchingArtists()
    {
        // Arrange
        const string prompt = "John";
        var expected = new List<Artist>
        {
            new Artist(Guid.NewGuid(), null, "john@example.com", "John", "Doe", "Painter", 5, "john.jpg")
        };

        _searchServiceMock
            .Setup(s => s.SearchArtistsByPromptAsync(prompt))
            .ReturnsAsync(expected);

        // Act
        var result = (await _searchServiceMock.Object.SearchArtistsByPromptAsync(prompt)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("John", result.First().FirstName);
    }

    [Fact]
    public async Task SearchEmployersByPromptAsync_ReturnsMatchingEmployers()
    {
        // Arrange
        const string prompt = "creative";
        var expected = new List<Employer>
        {
            new Employer(Guid.NewGuid(), "creative me", "asd@mail.ru", "sfjhsdjkf", "First", "Last")
        };

        _searchServiceMock
            .Setup(s => s.SearchEmployersByPromptAsync(prompt))
            .ReturnsAsync(expected);

        // Act
        var result = (await _searchServiceMock.Object.SearchEmployersByPromptAsync(prompt)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("creative me", result.First().CompanyName);
    }

    [Fact]
    public async Task SearchArtWorksByPromptAsync_ReturnsMatchingArtworks()
    {
        // Arrange
        const string prompt = "Abstract";
        var expected = new List<Artwork>
        {
            new Artwork(Guid.NewGuid(), Guid.NewGuid(), "Abstract Dreams", "A beautiful abstract piece", "we.png")
        };

        _searchServiceMock
            .Setup(s => s.SearchArtWorksByPromptAsync(prompt))
            .ReturnsAsync(expected);

        // Act
        var result = (await _searchServiceMock.Object.SearchArtWorksByPromptAsync(prompt)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Abstract Dreams", result.First().Title);
    }
}

