using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Portfolio;

public class CreatePortfolioDto(
    Guid artistId,
    string title,
    string? description,
    Guid techniqueId)
{
    [Required]
    [JsonPropertyName("artist_id")]
    public Guid ArtistId { get; set; } = artistId;

    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = title;

    [JsonPropertyName("description")]
    public string? Description { get; set; } = description;

    [Required]
    [JsonPropertyName("technique_id")]
    public Guid TechniqueId { get; set; } = techniqueId;
}
