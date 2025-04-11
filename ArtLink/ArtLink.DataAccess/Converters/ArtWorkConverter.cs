using System.Diagnostics.CodeAnalysis;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Models;

namespace ArtLink.DataAccess.Converters;

public static class ArtworkConverter
{
    [return: NotNullIfNotNull(nameof(artworkDb))]
    public static Artwork? ToDomain(this ArtworkDb? artworkDb)
    {
        if (artworkDb is null)
            return null;

        return new Artwork(
            id: artworkDb.Id,
            portfolioId: artworkDb.PortfolioId,
            title: artworkDb.Title,
            description: artworkDb.Description,
            imagePath: artworkDb.ImagePath
        );
    }
    
    [return: NotNullIfNotNull(nameof(artwork))]
    public static ArtworkDb? ToDataAccess(this Artwork? artwork)
    {
        if (artwork is null)
            return null;

        return new ArtworkDb(
            id: artwork.Id,
            portfolioId: artwork.PortfolioId,
            title: artwork.Title,
            description: artwork.Description,
            imagePath: artwork.ImagePath
        );
    }
}

