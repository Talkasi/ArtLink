using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.ArtWork;

public class CreateArtworkDto(
    Guid portfolioId,
    string title,
    string imagePath,
    string? description = null)
{
    [Required]
    [JsonPropertyName("portfolio_id")]
    public Guid PortfolioId { get; set; } = portfolioId;

    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = title;

    [JsonPropertyName("description")]
    public string? Description { get; set; } = description;

    [Required]
    [JsonPropertyName("image_path")]
    public string ImagePath { get; set; } = imagePath;
}
