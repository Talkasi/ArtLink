using System.Diagnostics.CodeAnalysis;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Models;

namespace ArtLink.DataAccess.Converters;

public static class PortfolioConverter
{
    [return: NotNullIfNotNull(nameof(portfolioDb))]
    public static Portfolio? ToDomain(this PortfolioDb? portfolioDb)
    {
        if (portfolioDb is null)
            return null;

        return new Portfolio(
            id: portfolioDb.Id,
            artistId: portfolioDb.ArtistId,
            techniqueId: portfolioDb.TechniqueId,
            title: portfolioDb.Title,
            description: portfolioDb.Description
        );
    }

    [return: NotNullIfNotNull(nameof(portfolio))]
    public static PortfolioDb? ToDataAccess(this Portfolio? portfolio)
    {
        if (portfolio is null)
            return null;

        return new PortfolioDb(
            id: portfolio.Id,
            artistId: portfolio.ArtistId,
            techniqueId: portfolio.TechniqueId,
            title: portfolio.Title,
            description: portfolio.Description
        );
    }
}

