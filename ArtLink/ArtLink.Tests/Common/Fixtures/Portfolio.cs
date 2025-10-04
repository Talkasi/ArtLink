using ArtLink.Domain.Models;
using AutoFixture;

namespace ArtLink.Tests.Common.Fixtures;

public class PortfolioFixture
{
    private readonly IFixture _fixture;

    public PortfolioFixture()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public Portfolio CreatePortfolio(
        Guid? id = null,
        Guid? artistId = null,
        string? title = null,
        Guid? techniqueId = null,
        string? description = null)
    {
        var portfolio = _fixture.Build<Portfolio>()
            .With(p => p.Id, id ?? Guid.NewGuid())
            .With(p => p.ArtistId, artistId ?? Guid.NewGuid())
            .With(p => p.Title, title ?? "Test Portfolio")
            .With(p => p.TechniqueId, techniqueId ?? Guid.NewGuid())
            .With(p => p.Description, description ?? "Test Description")
            .Create();

        return portfolio;
    }
}
