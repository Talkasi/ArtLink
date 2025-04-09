using System;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Portfolio;

public class PortfolioDto(
    Guid id,
    Guid artistId,
    string title,
    string description,
    Guid techniqueId)
{
    [JsonPropertyName("portfolio_id")]
    public Guid Id { get; set; } = id;

    [JsonPropertyName("artist_id")]
    public Guid ArtistId { get; set; } = artistId;

    [JsonPropertyName("title")]
    public string Title { get; set; } = title;

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;

    [JsonPropertyName("technique_id")]
    public Guid TechniqueId { get; set; } = techniqueId;
}
