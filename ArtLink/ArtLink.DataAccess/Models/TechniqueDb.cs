using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class TechniqueDb(Guid id, string name, string description)
{
    [Key]
    public Guid Id { get; init; } = id;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = name;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = description;

    public List<PortfolioDb> Portfolios { get; init; } = [];
}
