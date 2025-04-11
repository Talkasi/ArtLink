namespace ArtLink.Domain.Models;

public class Artwork(
    Guid id,
    Guid portfolioId,
    string title,
    string? description,
    string imagePath)
{
    public Guid Id { get; set; } = id;

    public Guid PortfolioId { get; set; } = portfolioId;

    public string Title { get; set; } = title;

    public string? Description { get; set; } = description;

    public string ImagePath { get; set; } = imagePath;
}
