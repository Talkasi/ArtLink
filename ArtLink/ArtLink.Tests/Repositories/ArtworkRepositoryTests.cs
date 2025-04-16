using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.Repositories;

public class ArtworkRepositoryTests
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
    public async Task AddAsync_ShouldAddArtwork()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);
        var portfolioId = Guid.NewGuid();

        await repository.AddAsync(portfolioId, "Test Title", "path/to/image.jpg", "Test Description");

        var artworks = await context.Artworks.ToListAsync();
        Assert.Single(artworks);
        Assert.Equal("Test Title", artworks[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnArtwork()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);
        var artworkId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();

        context.Artworks.Add(new ArtworkDb(
            artworkId, portfolioId, "Title", "path/to/image.jpg", "Description"));
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(artworkId);

        Assert.NotNull(result);
        Assert.Equal("Title", result.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateArtwork()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);
        var artworkId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();

        context.Artworks.Add(new ArtworkDb(
            artworkId, portfolioId, "Old Title", "old/path.jpg", "Old Description"));
        await context.SaveChangesAsync();

        await repository.UpdateAsync(artworkId, portfolioId, "New Title", "new/path.jpg", "New Description");

        var updated = await context.Artworks.FindAsync(artworkId);
        Assert.Equal("New Title", updated!.Title);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveArtwork()
    {
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);
        var artworkId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();

        var artwork = new ArtworkDb(
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
        await using var context = CreateContext();
        var repository = GetInMemoryRepository(context);
        var portfolioId = Guid.NewGuid();

        context.Artworks.AddRange(
            new ArtworkDb(Guid.NewGuid(), portfolioId, "Sunset", "path1.jpg", "A beautiful sunset"),
            new ArtworkDb(Guid.NewGuid(), portfolioId, "Mountain", "path2.jpg", "Snowy mountain")
        );
        await context.SaveChangesAsync();

        var results = await repository.SearchByPromptAsync("sun");

        var collection = results.ToList();
        Assert.Single(collection);
        Assert.Contains("Sunset", collection.First().Title);
    }
}
