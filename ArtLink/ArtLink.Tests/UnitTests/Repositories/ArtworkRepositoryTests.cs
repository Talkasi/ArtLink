using Allure.Xunit.Attributes;
using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.DataAccess.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.UnitTests.Repositories;

[AllureSuite("Artwork Repository Tests")]
public class ArtworkRepositoryUnitTests(ArtworkFixture fixture) : IClassFixture<ArtworkFixture>
{
    private static ArtLinkDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ArtLinkDbContext(options);
    }

    private static ArtworkRepository GetInMemoryRepository(ArtLinkDbContext context)
    {
        var logger = NullLogger<ArtworkRepository>.Instance;
        return new ArtworkRepository(context, logger);
    }

    [Fact]
    [AllureFeature("AddAsync + GetByIdAsync")]
    [AllureStory("Positive case - artwork added and retrieved")]
    public async Task AddAndGetById_ShouldReturnCorrectArtwork()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);

        var artwork = fixture.CreateArtwork();
        await repository.AddAsync(artwork.PortfolioId, artwork.Title, artwork.ImagePath, artwork.Description);

        var artworks = await context.Artworks.ToListAsync();
        var added = artworks.FirstOrDefault();

        var result = await repository.GetByIdAsync(added!.Id);

        result.Should().NotBeNull();
        result.Title.Should().Be(artwork.Title);
        result.Description.Should().Be(artwork.Description);
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Positive case - update artwork")]
    public async Task Update_ShouldModifyArtworkFields()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);

        var artwork = fixture.CreateArtwork(title: "Old Title", description: "Old Description");
        context.Artworks.Add(new ArtworkDb(artwork.Id, artwork.PortfolioId, artwork.Title, artwork.ImagePath, artwork.Description));
        await context.SaveChangesAsync();

        await repository.UpdateAsync(artwork.Id, artwork.PortfolioId, "New Title", "/new/path.jpg", "New Description");

        var updated = await context.Artworks.FindAsync(artwork.Id);
        updated!.Title.Should().Be("New Title");
        updated.Description.Should().Be("New Description");
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Positive case - delete artwork")]
    public async Task Delete_ShouldRemoveArtwork()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);

        var artwork = fixture.CreateArtwork();
        var artworkDb = new ArtworkDb(artwork.Id, artwork.PortfolioId, artwork.Title, artwork.ImagePath, artwork.Description);
        await context.Artworks.AddAsync(artworkDb);
        await context.SaveChangesAsync();

        await repository.DeleteAsync(artwork.Id);

        var deleted = await context.Artworks.FindAsync(artwork.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    [AllureFeature("SearchByPromptAsync")]
    [AllureStory("Positive case - search artworks")]
    public async Task SearchByPrompt_ShouldReturnMatchingArtworks()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);
        var portfolioId = Guid.NewGuid();

        var artworks = fixture.CreateArtworks(2, portfolioId);
        artworks[0] = fixture.CreateArtwork(portfolioId: portfolioId, title: "Sunset", description: "A beautiful sunset");
        artworks[1] = fixture.CreateArtwork(portfolioId: portfolioId, title: "Mountain", description: "Snowy mountain");

        context.Artworks.AddRange(artworks.Select(a => new ArtworkDb(a.Id, a.PortfolioId, a.Title, a.ImagePath, a.Description)));
        await context.SaveChangesAsync();

        var results = (await repository.SearchByPromptAsync("sun")).ToList();

        results.Should().HaveCount(1);
        results[0].Title.Should().Contain("Sunset");
    }

    [Fact]
    [AllureFeature("SearchByPromptAsync")]
    [AllureStory("Negative case - no matches")]
    public async Task SearchByPrompt_NoMatch_ShouldReturnEmpty()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);

        var artwork = fixture.CreateArtwork(title: "Starry Night");
        await repository.AddAsync(artwork.PortfolioId, artwork.Title, artwork.ImagePath, artwork.Description);

        var results = await repository.SearchByPromptAsync("Moon");
        results.Should().BeEmpty();
    }

    [Fact]
    [AllureFeature("GetByIdAsync")]
    [AllureStory("Exception handling - non-existent id get")]
    public async Task GetById_NonExistent_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Exception handling - non-existent id update")]
    public async Task Update_NonExistentId_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);

        Func<Task> act = async () => await repository.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), "x", "/x.jpg", "y");
        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Exception handling - non-existent id delete")]
    public async Task Delete_NonExistentId_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);

        Func<Task> act = async () => await repository.DeleteAsync(Guid.NewGuid());
        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("GetByPortfolioIdAsync")]
    [AllureStory("Positive case - get artworks by portfolio")]
    public async Task GetByPortfolioId_ShouldReturnPortfolioArtworks()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);

        var portfolioId = Guid.NewGuid();
        var otherPortfolioId = Guid.NewGuid();

        var portfolioArtworks = fixture.CreateArtworks(2, portfolioId);
        var otherArtwork = fixture.CreateArtwork(portfolioId: otherPortfolioId);

        foreach (var artwork in portfolioArtworks.Concat([otherArtwork]))
        {
            context.Artworks.Add(new ArtworkDb(artwork.Id, artwork.PortfolioId, artwork.Title, artwork.ImagePath, artwork.Description));
        }
        await context.SaveChangesAsync();

        var results = await repository.GetAllByPortfolioIdAsync(portfolioId);
        var enumerable = results as Artwork[] ?? results.ToArray();
        enumerable.Should().HaveCount(2);
        enumerable.All(a => a.PortfolioId == portfolioId).Should().BeTrue();
    }
}