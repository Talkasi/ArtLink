using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.Repositories;

public class TechniqueRepositoryTests
{
    private static ArtLinkDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ArtLinkDbContext(options);
    }

    private static TechniqueRepository CreateRepository(ArtLinkDbContext context)
    {
        return new TechniqueRepository(context, NullLogger<TechniqueRepository>.Instance);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTechnique()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);

        await repository.AddAsync("Oil Painting", "Thick layered paint");

        var technique = await context.Techniques.FirstOrDefaultAsync();
        Assert.NotNull(technique);
        Assert.Equal("Oil Painting", technique.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAll()
    {
        await using var context = CreateContext();
        context.Techniques.AddRange(
            new TechniqueDb(Guid.NewGuid(), "Watercolor", "Light pigment"),
            new TechniqueDb(Guid.NewGuid(), "Sketch", "Simple pencil")
        );
        await context.SaveChangesAsync();

        var repository = CreateRepository(context);
        var results = (await repository.GetAllAsync()).ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyTechnique()
    {
        await using var context = CreateContext();
        var id = Guid.NewGuid();
        context.Techniques.Add(new TechniqueDb(id, "Old", "Old Desc"));
        await context.SaveChangesAsync();

        var repository = CreateRepository(context);
        await repository.UpdateAsync(id, "New", "New Desc");

        var updated = await context.Techniques.FindAsync(id);
        Assert.Equal("New", updated!.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTechnique()
    {
        await using var context = CreateContext();
        var id = Guid.NewGuid();
        context.Techniques.Add(new TechniqueDb(id, "ToDelete", "Desc"));
        await context.SaveChangesAsync();

        var repository = CreateRepository(context);
        await repository.DeleteAsync(id);

        var deleted = await context.Techniques.FindAsync(id);
        Assert.Null(deleted);
    }
}

