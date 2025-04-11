using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtLink.Tests.Repositories;

public class ArtworkRepositoryTests
{
    private ArtLinkDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ArtLinkDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddArtwork()
    {
        using var context = CreateContext();
        var repository = new ArtworkRepository(context);
        var portfolioId = Guid.NewGuid();

        await repository.AddAsync(portfolioId, "Test Title", "Test Description", "path/to/image.jpg");

        var artworks = await context.Artworks.ToListAsync();
        Assert.Single(artworks);
        Assert.Equal("Test Title", artworks[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnArtwork()
    {
        using var context = CreateContext();
        var repository = new ArtworkRepository(context);
        var artworkId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();

        context.Artworks.Add(new DataAccess.Models.ArtworkDb(
            artworkId, portfolioId, "Title", "path/to/image.jpg", "Description"));
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(artworkId);

        Assert.NotNull(result);
        Assert.Equal("Title", result!.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateArtwork()
    {
        using var context = CreateContext();
        var repository = new ArtworkRepository(context);
        var artworkId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();

        context.Artworks.Add(new DataAccess.Models.ArtworkDb(
            artworkId, portfolioId, "Old Title", "old/path.jpg", "Old Description"));
        await context.SaveChangesAsync();

        await repository.UpdateAsync(artworkId, portfolioId, "New Title", "New Description", "new/path.jpg");

        var updated = await context.Artworks.FindAsync(artworkId);
        Assert.Equal("New Title", updated!.Title);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveArtwork()
    {
        using var context = CreateContext();
        var repository = new ArtworkRepository(context);
        var artworkId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();

        var artwork = new DataAccess.Models.ArtworkDb(
            artworkId, portfolioId, "Title", "path/to/image.jpg", "Description");

        await context.Artworks.AddAsync(artwork);
        await context.SaveChangesAsync();

        await repository.DeleteAsync(artworkId);

        var result = await context.Artworks.FindAsync(artworkId);
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchByPromptAsync_ShouldReturnMatchingArtworks()
    {
        using var context = CreateContext();
        var repository = new ArtworkRepository(context);
        var portfolioId = Guid.NewGuid();

        context.Artworks.AddRange(
            new DataAccess.Models.ArtworkDb(Guid.NewGuid(), portfolioId, "Sunset", "path1.jpg", "A beautiful sunset"),
            new DataAccess.Models.ArtworkDb(Guid.NewGuid(), portfolioId, "Mountain", "path2.jpg", "Snowy mountain")
        );
        await context.SaveChangesAsync();

        var results = await repository.SearchByPromptAsync("sun");

        Assert.Single(results);
        Assert.Contains("Sunset", results.First().Title);
    }
}

