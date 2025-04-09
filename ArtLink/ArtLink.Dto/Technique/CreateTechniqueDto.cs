using System.Text.Json.Serialization;

namespace ArtLink.Dto.Technique;

public class CreateTechniqueDto(string name, string description)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;
}
