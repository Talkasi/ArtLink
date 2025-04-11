using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtLink.Tests.Repositories;

public class PortfolioRepositoryTests
{
    private ArtLinkDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ArtLinkDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddPortfolio()
    {
        using var context = GetInMemoryDbContext();
        var repository = new PortfolioRepository(context);

        var artistId = Guid.NewGuid();
        var techniqueId = Guid.NewGuid();
        var title = "Test Portfolio";
        var description = "Test Description";

        await repository.AddAsync(artistId, title, description, techniqueId);

        var portfolio = await context.Portfolios.FirstOrDefaultAsync();
        Assert.NotNull(portfolio);
        Assert.Equal(title, portfolio.Title);
        Assert.Equal(description, portfolio.Description);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPortfolio()
    {
        using var context = GetInMemoryDbContext();
        var portfolioId = Guid.NewGuid();
        var portfolioDb = new PortfolioDb(portfolioId, Guid.NewGuid(), Guid.NewGuid(), "Title", "Desc");
        context.Portfolios.Add(portfolioDb);
        await context.SaveChangesAsync();

        var repository = new PortfolioRepository(context);
        var result = await repository.GetByIdAsync(portfolioId);

        Assert.NotNull(result);
        Assert.Equal(portfolioId, result!.Id);
    }

    [Fact]
    public async Task GetAllByArtistIdAsync_ShouldReturnCorrectPortfolios()
    {
        using var context = GetInMemoryDbContext();
        var artistId = Guid.NewGuid();
        context.Portfolios.AddRange(
            new PortfolioDb(Guid.NewGuid(), artistId, Guid.NewGuid(), "Title 1", "desc 1"),
            new PortfolioDb(Guid.NewGuid(), artistId, Guid.NewGuid(), "Title 2", "desc 2"),
            new PortfolioDb(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Other Artist", "desc 3")
        );
        await context.SaveChangesAsync();

        var repository = new PortfolioRepository(context);
        var result = await repository.GetAllByArtistIdAsync(artistId);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePortfolio()
    {
        using var context = GetInMemoryDbContext();
        var portfolioId = Guid.NewGuid();
        var portfolio = new PortfolioDb(portfolioId, Guid.NewGuid(), Guid.NewGuid(), "Old Title", "Old Desc");
        context.Portfolios.Add(portfolio);
        await context.SaveChangesAsync();

        var repository = new PortfolioRepository(context);
        var newTitle = "New Title";
        var newDesc = "New Desc";
        var newTechniqueId = Guid.NewGuid();
        var newArtistId = Guid.NewGuid();

        await repository.UpdateAsync(portfolioId, newArtistId, newTitle, newDesc, newTechniqueId);

        var updated = await context.Portfolios.FindAsync(portfolioId);
        Assert.NotNull(updated);
        Assert.Equal(newTitle, updated!.Title);
        Assert.Equal(newDesc, updated.Description);
        Assert.Equal(newTechniqueId, updated.TechniqueId);
        Assert.Equal(newArtistId, updated.ArtistId);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemovePortfolio()
    {
        using var context = GetInMemoryDbContext();
        var portfolioId = Guid.NewGuid();
        context.Portfolios.Add(new PortfolioDb(portfolioId, Guid.NewGuid(), Guid.NewGuid(), "Title", "asd"));
        await context.SaveChangesAsync();

        var repository = new PortfolioRepository(context);
        await repository.DeleteAsync(portfolioId);

        var deleted = await context.Portfolios.FindAsync(portfolioId);
        Assert.Null(deleted);
    }
}
