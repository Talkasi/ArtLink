using System.Text.Json.Serialization;

namespace ArtLink.Dto.Technique;

public class TechniqueDto(
    Guid id,
    string name,
    string description)
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = id;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;
}
