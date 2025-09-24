using ArtLink.Domain.Models;
using AutoFixture;

namespace ArtLink.Tests.Fixtures;

public class ArtistFixture
{
    private readonly IFixture _fixture;

    public ArtistFixture()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public Artist CreateArtist(
        Guid? id = null,
        string? email = null,
        string? firstName = null,
        string? lastName = null)
    {
        var artist = _fixture.Build<Artist>()
            .With(a => a.Id, id ?? Guid.NewGuid())
            .With(a => a.Email, email ?? "test@example.com")
            .With(a => a.FirstName, firstName ?? "Test")
            .With(a => a.LastName, lastName ?? "Artist")
            .With(a => a.PasswordHash, "hashed_password")
            .Create();

        return artist;
    }
}
