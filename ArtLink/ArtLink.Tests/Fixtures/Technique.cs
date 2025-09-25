using ArtLink.Domain.Models;
using AutoFixture;

namespace ArtLink.Tests.Fixtures;

public class TechniqueFixture
{
    private readonly IFixture _fixture;

    public TechniqueFixture()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public Technique CreateTechnique(
        Guid? id = null,
        string? name = null,
        string? description = null)
    {
        var technique = _fixture.Build<Technique>()
            .With(t => t.Id, id ?? Guid.NewGuid())
            .With(t => t.Name, name ?? "Test Technique")
            .With(t => t.Description, description ?? "Test Description")
            .Create();

        return technique;
    }
}
