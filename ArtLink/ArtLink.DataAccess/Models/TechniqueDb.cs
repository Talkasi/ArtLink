using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class TechniqueDb
{
    public TechniqueDb(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; }

    public List<PortfolioDb> Portfolios { get; init; } = [];
}
