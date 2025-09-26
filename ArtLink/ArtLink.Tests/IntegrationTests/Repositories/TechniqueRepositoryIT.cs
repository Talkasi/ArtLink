using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Tests.Common.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Repositories;

[Collection("Database collection")]
public class TechniqueRepositoryIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private TechniqueRepository CreateRepository()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        return new TechniqueRepository(context, new NullLogger<TechniqueRepository>());
    }

    [Fact]
    public async Task Technique_FullCycle()
    {
        // Arrange
        var repo = CreateRepository();
        var name = "Test Technique";
        var description = "Test description";

        // Act 1 — добавить технику
        var techniqueId = await repo.AddAsync(name, description);

        // Assert 1 — проверить что добавлено
        var allTechniques = await repo.GetAllAsync();
        allTechniques.Should().ContainSingle(t => t.Id == techniqueId && t.Name == name);

        // Act 2 — обновить
        var updatedName = "Updated Technique";
        var updatedDescription = "Updated description";
        await repo.UpdateAsync(techniqueId, updatedName, updatedDescription);

        // Assert 2 — проверить обновление
        var updatedTechnique = (await repo.GetAllAsync()).FirstOrDefault(t => t.Id == techniqueId);
        updatedTechnique.Should().NotBeNull();
        updatedTechnique.Name.Should().Be(updatedName);
        updatedTechnique.Description.Should().Be(updatedDescription);

        // Act 3 — удалить
        await repo.DeleteAsync(techniqueId);

        // Assert 3 — проверить удаление
        var afterDelete = await repo.GetAllAsync();
        afterDelete.Should().NotContain(t => t.Id == techniqueId);
    }
}
