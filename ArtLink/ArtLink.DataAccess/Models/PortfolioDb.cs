using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class PortfolioDb(
    Guid id,
    Guid artistId,
    Guid techniqueId,
    string title,
    string? description = null)
{
    [Key]
    public Guid Id { get; init; } = id;

    [Required]
    public Guid ArtistId { get; set; } = artistId;

    [Required]
    public Guid TechniqueId { get; set; } = techniqueId;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = title;

    [MaxLength(2000)]
    public string? Description { get; set; } = description;

    public ArtistDb? Artist { get; init; }
    public TechniqueDb? Technique { get; init; }
    
    public List<ArtworkDb> Artworks { get; init; } = [];
}
