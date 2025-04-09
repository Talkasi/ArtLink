using System.Text.Json.Serialization;

namespace ArtLink.Dto.Portfolio;

public class CreatePortfolioDto(
    Guid artistId,
    string title,
    string description,
    Guid techniqueId)
{
    [JsonPropertyName("artist_id")]
    public Guid ArtistId { get; set; } = artistId;

    [JsonPropertyName("title")]
    public string Title { get; set; } = title;

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;

    [JsonPropertyName("technique_id")]
    public Guid TechniqueId { get; set; } = techniqueId;
}
