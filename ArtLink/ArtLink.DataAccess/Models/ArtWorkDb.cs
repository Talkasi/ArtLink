using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class ArtworkDb
{
    public ArtworkDb(Guid id, 
        Guid portfolioId, 
        string title, 
        string imagePath,
        string? description = null)
    {
        Id = id;
        PortfolioId = portfolioId;
        Title = title;
        Description = description;
        ImagePath = imagePath;
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid PortfolioId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(500)]
    public string ImagePath { get; set; }

    public PortfolioDb? Portfolio { get; set; }
}
