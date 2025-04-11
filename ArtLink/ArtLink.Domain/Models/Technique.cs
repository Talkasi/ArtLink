namespace ArtLink.Domain.Models;

public class Technique(
    Guid id,
    string name,
    string description)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    public string Description { get; set; } = description;
}
