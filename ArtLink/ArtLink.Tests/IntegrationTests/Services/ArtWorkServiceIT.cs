using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.DataAccess.Repositories;
using ArtLink.Services.Artwork;
using ArtLink.Tests.Common;
using ArtLink.Tests.Common.Database;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Services;

[Collection("Database collection")]
[Trait("Category", TestCategories.Integration)]
public class ArtworkServiceIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private readonly ArtworkFixture _artworkFixture = new ArtworkFixture();
    private readonly PortfolioFixture _portfolioFixture = new PortfolioFixture();

    private ArtworkService CreateService()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        var repository = new ArtworkRepository(context, new NullLogger<ArtworkRepository>());
        return new ArtworkService(repository, new NullLogger<ArtworkService>());
    }

    [Fact]
    public async Task Artwork_FullCycle_WithFixtures()
    {
        var service = CreateService();

        var artistId = Guid.NewGuid();
        var techniqueId = Guid.NewGuid();
        var portfolio = _portfolioFixture.CreatePortfolio(
            artistId: artistId,
            techniqueId: techniqueId
        );

        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        await using (var context = new ArtLinkDbContext(options))
        {
            await context.Artists.AddAsync(new ArtistDb(artistId, "hashed", "artist@example.com", "Test", "Artist"));
            await context.Techniques.AddAsync(new TechniqueDb(techniqueId, "Oil", "Oil painting"));
            await context.Portfolios.AddAsync(new PortfolioDb(portfolio.Id, artistId, techniqueId, portfolio.Title, portfolio.Description));
            await context.SaveChangesAsync();
        }

        var artwork = _artworkFixture.CreateArtwork(portfolioId: portfolio.Id);
        
        var newId = await service.AddArtworkAsync(artwork.PortfolioId, artwork.Title, artwork.ImagePath, artwork.Description);

        var added = await service.GetArtworkByIdAsync(newId);
        added.Should().NotBeNull();
        added.Title.Should().Be(artwork.Title);

        await service.UpdateArtworkAsync(newId, artwork.PortfolioId, "Updated Title", artwork.ImagePath, "Updated Description");

        var updated = await service.GetArtworkByIdAsync(newId);
        updated.Should().NotBeNull();
        updated.Title.Should().Be("Updated Title");
        updated.Description.Should().Be("Updated Description");

        var allByPortfolio = await service.GetAllByPortfolioIdAsync(portfolio.Id);
        allByPortfolio.Should().Contain(c => c.Id == newId);

        await service.DeleteArtworkAsync(newId);
        var deleted = await service.GetArtworkByIdAsync(newId);
        deleted.Should().BeNull();
    }
}
