using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Services.Portfolio;
using ArtLink.Tests.Common;
using ArtLink.Tests.Common.Database;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Services;

[Collection("Database collection")]
[Trait("Category", TestCategories.Integration)]
public class PortfolioServiceIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private readonly PortfolioFixture _fixture = new PortfolioFixture();

    private PortfolioService CreateService()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        var repository = new PortfolioRepository(context, new NullLogger<PortfolioRepository>());
        return new PortfolioService(repository, new NullLogger<PortfolioService>());
    }

    [Fact]
    public async Task Portfolio_FullCycle_UsingFixture()
    {
        var service = CreateService();

        var artistId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var techniqueId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var testPortfolio = _fixture.CreatePortfolio(artistId: artistId, techniqueId: techniqueId);

        var portfolioId = await service.AddPortfolioAsync(
            testPortfolio.ArtistId,
            testPortfolio.Title,
            testPortfolio.TechniqueId,
            testPortfolio.Description
        );

        var added = await service.GetPortfolioByIdAsync(portfolioId);
        added.Should().NotBeNull();
        added.Title.Should().Be(testPortfolio.Title);

        await service.UpdatePortfolioAsync(
            portfolioId,
            artistId,
            testPortfolio.Title + " Updated",
            techniqueId,
            testPortfolio.Description
        );

        var updated = await service.GetPortfolioByIdAsync(portfolioId);
        updated?.Title.Should().Be(testPortfolio.Title + " Updated");

        var allPortfolios = await service.GetAllByArtistIdAsync(artistId);
        allPortfolios.Should().Contain(p => p.Id == portfolioId);

        await service.DeletePortfolioAsync(portfolioId);

        var deleted = await service.GetPortfolioByIdAsync(portfolioId);
        deleted.Should().BeNull();
    }
}
