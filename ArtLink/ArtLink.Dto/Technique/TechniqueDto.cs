using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Technique;

public class TechniqueDto(
    Guid id,
    string name,
    string description)
{
    [Required]
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = id;

    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [Required]
    [JsonPropertyName("description")]
    public string Description { get; set; } = description;
}
