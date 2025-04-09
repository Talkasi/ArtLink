using System;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.ArtWork;

public class CreateArtworkDto(
    Guid portfolioId,
    string title,
    string description,
    string imagePath)
{
    [JsonPropertyName("portfolio_id")]
    public Guid PortfolioId { get; set; } = portfolioId;

    [JsonPropertyName("title")]
    public string Title { get; set; } = title;

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;

    [JsonPropertyName("image_path")]
    public string ImagePath { get; set; } = imagePath;
}
