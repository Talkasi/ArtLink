using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Technique;

public class CreateTechniqueDto(string name, string description)
{
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [Required]
    [JsonPropertyName("description")]
    public string Description { get; set; } = description;
}
