using ArtLink.Domain.Models;
using AutoFixture;

namespace ArtLink.Tests.Fixtures;

public class ArtworkFixture
{
    private readonly IFixture _fixture;

    public ArtworkFixture()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public Artwork CreateArtwork(
        Guid? id = null,
        Guid? portfolioId = null,
        string? title = null,
        string? imagePath = null,
        string? description = null)
    {
        var artwork = _fixture.Build<Artwork>()
            .With(a => a.Id, id ?? Guid.NewGuid())
            .With(a => a.PortfolioId, portfolioId ?? Guid.NewGuid())
            .With(a => a.Title, title ?? "Test Artwork")
            .With(a => a.ImagePath, imagePath ?? "/test.png")
            .With(a => a.Description, description ?? "Test description")
            .Create();

        return artwork;
    }

    public List<Artwork> CreateArtworks(int count, Guid? portfolioId = null)
    {
        var artworks = new List<Artwork>();
        for (int i = 0; i < count; i++)
        {
            artworks.Add(CreateArtwork(
                portfolioId: portfolioId,
                title: $"Artwork {i + 1}",
                description: $"Description for artwork {i + 1}"));
        }
        return artworks;
    }
}
