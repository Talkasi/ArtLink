using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Services.Technique;
using ArtLink.Tests.Common.Database;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Services;

[Collection("Database collection")]
public class TechniqueServiceIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private readonly TechniqueFixture _fixture = new TechniqueFixture();

    private TechniqueService CreateService()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        var repository = new TechniqueRepository(context, new NullLogger<TechniqueRepository>());
        return new TechniqueService(repository, new NullLogger<TechniqueService>());
    }

    [Fact]
    public async Task Technique_FullCycle_UsingFixture()
    {
        // Arrange
        var service = CreateService();
        var testTechnique = _fixture.CreateTechnique();

        // Act 1 — добавить технику
        var id = await service.AddTechniqueAsync(testTechnique.Name, testTechnique.Description);

        // Assert 1 — проверить добавление
        var added = (await service.GetAllAsync()).FirstOrDefault(t => t.Id == id);
        added.Should().NotBeNull();
        added.Name.Should().Be(testTechnique.Name);
        added.Description.Should().Be(testTechnique.Description);

        // Act 2 — обновить
        var updatedName = testTechnique.Name + " Updated";
        var updatedDescription = testTechnique.Description + " Updated";
        await service.UpdateTechniqueAsync(id, updatedName, updatedDescription);

        // Assert 2 — проверить обновление
        var updated = (await service.GetAllAsync()).FirstOrDefault(t => t.Id == id);
        updated.Should().NotBeNull();
        updated.Name.Should().Be(updatedName);
        updated.Description.Should().Be(updatedDescription);

        // Act 3 — удалить
        await service.DeleteTechniqueAsync(id);

        // Assert 3 — проверить удаление
        var deleted = (await service.GetAllAsync()).FirstOrDefault(t => t.Id == id);
        deleted.Should().BeNull();
    }
}
