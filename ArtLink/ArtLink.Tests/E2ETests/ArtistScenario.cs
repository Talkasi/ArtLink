using ArtLink.Domain.Models;
using ArtLink.Services.Artist;
using ArtLink.Services.Artwork;
using ArtLink.Services.Portfolio;
using ArtLink.Services.Technique;
using ArtLink.Tests.Common;
using ArtLink.Tests.Common.Database;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.E2ETests;

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}

[Collection("Database collection")]
[Trait("Category", TestCategories.E2E)]
public class ArtistScenarioE2ETest(DatabaseFixture db)
{
    private readonly ArtistFixture _artistFixture = new ArtistFixture();
    private readonly TechniqueFixture _techniqueFixture = new TechniqueFixture();
    private readonly PortfolioFixture _portfolioFixture = new PortfolioFixture();
    private readonly ArtworkFixture _artworkFixture = new ArtworkFixture();

    [Fact]
    public async Task DemoScenario_FullFlow()
    {
        var options = new DbContextOptionsBuilder<DataAccess.Context.ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new DataAccess.Context.ArtLinkDbContext(options);

        var artistService = new ArtistService(
            new DataAccess.Repositories.ArtistRepository(context, new NullLogger<DataAccess.Repositories.ArtistRepository>()),
            new NullLogger<ArtistService>());

        var techniqueService = new TechniqueService(
            new DataAccess.Repositories.TechniqueRepository(context, new NullLogger<DataAccess.Repositories.TechniqueRepository>()),
            new NullLogger<TechniqueService>());

        var portfolioService = new PortfolioService(
            new DataAccess.Repositories.PortfolioRepository(context, new NullLogger<DataAccess.Repositories.PortfolioRepository>()),
            new NullLogger<PortfolioService>());

        var artworkService = new ArtworkService(
            new DataAccess.Repositories.ArtworkRepository(context, new NullLogger<DataAccess.Repositories.ArtworkRepository>()),
            new NullLogger<ArtworkService>());

        var artist = _artistFixture.CreateArtist();
        var artistId = await artistService.AddArtistAsync(
            artist.FirstName, artist.LastName, artist.Email, artist.PasswordHash!, artist.Bio, artist.ProfilePicturePath, artist.Experience);

        var technique = _techniqueFixture.CreateTechnique();
        var techniqueId = await techniqueService.AddTechniqueAsync(technique.Name, technique.Description);

        var portfolio = _portfolioFixture.CreatePortfolio(artistId: artistId, techniqueId: techniqueId);
        var portfolioId = await portfolioService.AddPortfolioAsync(portfolio.ArtistId, portfolio.Title, portfolio.TechniqueId, portfolio.Description);

        var artworks = _artworkFixture.CreateArtworks(3, portfolioId);
        var artworkIds = new List<Guid>();
        foreach (var art in artworks)
        {
            var id = await artworkService.AddArtworkAsync(portfolioId, art.Title, art.ImagePath, art.Description);
            artworkIds.Add(id);
        }

        var savedArtworks = await artworkService.GetAllByPortfolioIdAsync(portfolioId);
        var enumerable = savedArtworks as Artwork[] ?? savedArtworks.ToArray();
        enumerable.Should().HaveCount(3);
        enumerable.Select(a => a.Title).Should().Contain(artworks.Select(a => a.Title));

        foreach (var artId in artworkIds)
            await artworkService.DeleteArtworkAsync(artId);

        await portfolioService.DeletePortfolioAsync(portfolioId);
        await artistService.DeleteArtistAsync(artistId);
        await techniqueService.DeleteTechniqueAsync(techniqueId);

        (await artworkService.GetAllByPortfolioIdAsync(portfolioId)).Should().BeEmpty();
        (await portfolioService.GetAllByArtistIdAsync(artistId)).Should().BeEmpty();
    }
}
