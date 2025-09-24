using Allure.Xunit.Attributes;
using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.DataAccess.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.UnitTests.Repositories;

[AllureSuite("Portfolio Repository Tests")]
public class PortfolioRepositoryUnitTests(PortfolioFixture fixture) : IClassFixture<PortfolioFixture>
{
    private static ArtLinkDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ArtLinkDbContext(options);
    }

    private static PortfolioRepository CreateRepository(ArtLinkDbContext context)
    {
        return new PortfolioRepository(context, NullLogger<PortfolioRepository>.Instance);
    }

    [Fact]
    [AllureFeature("AddAsync")]
    [AllureStory("Positive case - add portfolio")]
    public async Task AddAsync_ShouldAddPortfolio()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var portfolio = fixture.CreatePortfolio(
            title: "My Art Portfolio",
            description: "Collection of my best works");

        var id = await repo.AddAsync(
            portfolio.ArtistId,
            portfolio.Title,
            portfolio.TechniqueId,
            portfolio.Description);

        var added = await context.Portfolios.FindAsync(id);
        added.Should().NotBeNull();
        added.Title.Should().Be("My Art Portfolio");
        added.Description.Should().Be("Collection of my best works");
    }

    [Fact]
    [AllureFeature("GetByIdAsync")]
    [AllureStory("Positive case - get portfolio by id")]
    public async Task GetByIdAsync_ShouldReturnPortfolio()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var portfolio = fixture.CreatePortfolio(title: "Summer Collection");
        await context.Portfolios.AddAsync(new PortfolioDb(
            portfolio.Id,
            portfolio.ArtistId,
            portfolio.TechniqueId,
            portfolio.Title,
            portfolio.Description));
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(portfolio.Id);

        result.Should().NotBeNull();
        result.Title.Should().Be("Summer Collection");
        result.ArtistId.Should().Be(portfolio.ArtistId);
    }

    [Fact]
    [AllureFeature("GetByArtistIdAsync")]
    [AllureStory("Positive case - get portfolios by artist")]
    public async Task GetByArtistIdAsync_ShouldReturnArtistPortfolios()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var artistId = Guid.NewGuid();
        var otherArtistId = Guid.NewGuid();

        var artistPortfolios = new List<Portfolio>
        {
            fixture.CreatePortfolio(artistId: artistId, title: "Portfolio 1"),
            fixture.CreatePortfolio(artistId: artistId, title: "Portfolio 2")
        };
        
        var otherPortfolio = fixture.CreatePortfolio(artistId: otherArtistId, title: "Other Portfolio");

        await context.Portfolios.AddRangeAsync(artistPortfolios.Concat([otherPortfolio])
            .Select(p => new PortfolioDb(p.Id, p.ArtistId, p.TechniqueId,  p.Title, p.Description)));
        await context.SaveChangesAsync();

        var results = await repo.GetAllByArtistIdAsync(artistId);

        var portfolios = results as Portfolio[] ?? results.ToArray();
        portfolios.Should().HaveCount(2);
        portfolios.All(p => p.ArtistId == artistId).Should().BeTrue();
        portfolios.Should().Contain(p => p.Title == "Portfolio 1");
        portfolios.Should().Contain(p => p.Title == "Portfolio 2");
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Positive case - update portfolio")]
    public async Task UpdateAsync_ShouldModifyPortfolio()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var portfolio = fixture.CreatePortfolio(
            title: "Old Title",
            description: "Old Description");
            
        await context.Portfolios.AddAsync(new PortfolioDb(
            portfolio.Id,
            portfolio.ArtistId,
            portfolio.TechniqueId,
            portfolio.Title,
            portfolio.Description));
        await context.SaveChangesAsync();

        var newTechniqueId = Guid.NewGuid();
        await repo.UpdateAsync(
            portfolio.Id,
            portfolio.ArtistId,
            "New Title",
            newTechniqueId,
            "New Description");

        var updated = await context.Portfolios.FindAsync(portfolio.Id);
        updated.Should().NotBeNull();
        updated.Title.Should().Be("New Title");
        updated.Description.Should().Be("New Description");
        updated.TechniqueId.Should().Be(newTechniqueId);
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Positive case - delete portfolio")]
    public async Task DeleteAsync_ShouldRemovePortfolio()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var portfolio = fixture.CreatePortfolio(title: "To Delete");
        await context.Portfolios.AddAsync(new PortfolioDb(
            portfolio.Id,
            portfolio.ArtistId,
            portfolio.TechniqueId,
            portfolio.Title,
            portfolio.Description));
        await context.SaveChangesAsync();

        await repo.DeleteAsync(portfolio.Id);

        var deleted = await context.Portfolios.FindAsync(portfolio.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    [AllureFeature("GetByIdAsync")]
    [AllureStory("Negative case - non-existent portfolio")]
    public async Task GetByIdAsync_NonExistent_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    [AllureFeature("GetByArtistIdAsync")]
    [AllureStory("Negative case - no portfolios for artist")]
    public async Task GetByArtistIdAsync_NoPortfolios_ShouldReturnEmpty()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var results = await repo.GetAllByArtistIdAsync(Guid.NewGuid());
        
        results.Should().BeEmpty();
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Exception handling - update non-existent portfolio")]
    public async Task UpdateAsync_NonExistent_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        Func<Task> act = async () => await repo.UpdateAsync(
            Guid.NewGuid(), Guid.NewGuid(), "New Title", Guid.NewGuid(), "New Description");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Exception handling - delete non-existent portfolio")]
    public async Task DeleteAsync_NonExistent_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        Func<Task> act = async () => await repo.DeleteAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }
}