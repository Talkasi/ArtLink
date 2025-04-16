using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class ArtworkDb(
    Guid id,
    Guid portfolioId,
    string title,
    string imagePath,
    string? description = null)
{
    [Key]
    public Guid Id { get; init; } = id;

    [Required]
    public Guid PortfolioId { get; set; } = portfolioId;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = title;

    [MaxLength(2000)]
    public string? Description { get; set; } = description;

    [Required]
    [MaxLength(500)]
    public string ImagePath { get; set; } = imagePath;

    public PortfolioDb? Portfolio { get; init; }
}
