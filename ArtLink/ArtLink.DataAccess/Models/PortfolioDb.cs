using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class PortfolioDb
{
    public PortfolioDb(Guid id,
        Guid artistId,
        Guid techniqueId,
        string title,
        string? description = null)
    {
        Id = id;
        ArtistId = artistId;
        TechniqueId = techniqueId;
        Title = title;
        Description = description;
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ArtistId { get; set; }

    [Required]
    public Guid TechniqueId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public ArtistDb? Artist { get; set; }
    public TechniqueDb? Technique { get; set; }

    public List<ArtworkDb> Artworks { get; init; } = [];
}
