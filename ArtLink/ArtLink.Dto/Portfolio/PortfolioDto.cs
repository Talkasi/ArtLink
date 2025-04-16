using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Portfolio;

public class PortfolioDto(
    Guid id,
    Guid artistId,
    string title,
    Guid techniqueId,
    string? description = null)
{
    [Required]
    [JsonPropertyName("portfolio_id")]
    public Guid Id { get; set; } = id;

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
