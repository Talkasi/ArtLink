namespace ArtLink.Domain.Models;

public class Portfolio(
    Guid id,
    Guid artistId,
    Guid techniqueId,
    string title,
    string? description)
{
    public Guid Id { get; set; } = id;

    public Guid ArtistId { get; set; } = artistId;

    public Guid TechniqueId { get; set; } = techniqueId;

    public string Title { get; set; } = title;

    public string? Description { get; set; } = description;
}
