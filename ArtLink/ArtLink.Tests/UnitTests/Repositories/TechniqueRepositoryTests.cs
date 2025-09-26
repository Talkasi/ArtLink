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

[AllureSuite("Technique Repository Tests")]
public class TechniqueRepositoryUnitTests(TechniqueFixture fixture) : IClassFixture<TechniqueFixture>
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
    [AllureFeature("AddAsync")]
    [AllureStory("Positive case - add technique")]
    [AllureDescription("Проверка добавления техники в репозиторий")]
    public async Task AddAsync_ShouldAddTechnique()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var technique = fixture.CreateTechnique(
            name: "Oil Painting", 
            description: "Thick layered paint");

        var id = await repo.AddAsync(technique.Name, technique.Description);

        var added = await context.Techniques.FindAsync(id);
        added.Should().NotBeNull();
        added.Name.Should().Be("Oil Painting");
        added.Description.Should().Be("Thick layered paint");
    }

    [Fact]
    [AllureFeature("GetAllAsync")]
    [AllureStory("Positive case - get all techniques")]
    [AllureDescription("Проверка получения всех техник из репозитория")]
    public async Task GetAllAsync_ShouldReturnAllTechniques()
    {
        await using var context = CreateContext();
        
        var techniques = new List<Technique>
        {
            fixture.CreateTechnique(name: "Watercolor", description: "Light pigment"),
            fixture.CreateTechnique(name: "Sketch", description: "Simple pencil")
        };

        await context.Techniques.AddRangeAsync(techniques.Select(t => 
            new TechniqueDb(t.Id, t.Name, t.Description)));
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);
        var results = (await repo.GetAllAsync()).ToList();

        results.Should().HaveCount(2);
        results.Should().Contain(t => t.Name == "Watercolor");
        results.Should().Contain(t => t.Name == "Sketch");
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Positive case - update technique")]
    [AllureDescription("Проверка обновления техники в репозитории")]
    public async Task UpdateAsync_ShouldModifyTechnique()
    {
        await using var context = CreateContext();
        
        var technique = fixture.CreateTechnique(name: "Old", description: "Old Desc");
        await context.Techniques.AddAsync(new TechniqueDb(technique.Id, technique.Name, technique.Description));
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);
        await repo.UpdateAsync(technique.Id, "New", "New Desc");

        var updated = await context.Techniques.FindAsync(technique.Id);
        updated.Should().NotBeNull();
        updated.Name.Should().Be("New");
        updated.Description.Should().Be("New Desc");
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Positive case - delete technique")]
    [AllureDescription("Проверка удаления техники из репозитория")]
    public async Task DeleteAsync_ShouldRemoveTechnique()
    {
        await using var context = CreateContext();
        
        var technique = fixture.CreateTechnique(name: "ToDelete", description: "Desc");
        await context.Techniques.AddAsync(new TechniqueDb(technique.Id, technique.Name, technique.Description));
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);
        await repo.DeleteAsync(technique.Id);

        var deleted = await context.Techniques.FindAsync(technique.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    [AllureFeature("GetByIdAsync")]
    [AllureStory("Positive case - get technique by id")]
    [AllureDescription("Проверка получения техники по идентификатору")]
    public async Task GetByIdAsync_ShouldReturnTechnique()
    {
        await using var context = CreateContext();
        
        var technique = fixture.CreateTechnique(name: "Digital Art", description: "Computer-generated art");
        await context.Techniques.AddAsync(new TechniqueDb(technique.Id, technique.Name, technique.Description));
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);
        var result = (await repo.GetAllAsync()).Last();

        result.Should().NotBeNull();
        result.Name.Should().Be("Digital Art");
        result.Description.Should().Be("Computer-generated art");
    }

    [Fact]
    [AllureFeature("GetByIdAsync")]
    [AllureStory("Negative case - non-existent technique")]
    [AllureDescription("Проверка возврата null для несуществующей техники")]
    public async Task GetByIdAsync_NonExistent_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Exception handling - update non-existent technique")]
    [AllureDescription("Проверка обработки обновления несуществующей техники")]
    public async Task UpdateAsync_NonExistent_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        Func<Task> act = async () => await repo.UpdateAsync(Guid.NewGuid(), "Name", "Description");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Exception handling - delete non-existent technique")]
    [AllureDescription("Проверка обработки удаления несуществующей техники")]
    public async Task DeleteAsync_NonExistent_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        Func<Task> act = async () => await repo.DeleteAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("GetAllAsync")]
    [AllureStory("Edge case - empty repository")]
    [AllureDescription("Проверка получения пустого списка техник")]
    public async Task GetAllAsync_EmptyRepository_ShouldReturnEmptyList()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var results = await repo.GetAllAsync();

        results.Should().BeEmpty();
    }
}