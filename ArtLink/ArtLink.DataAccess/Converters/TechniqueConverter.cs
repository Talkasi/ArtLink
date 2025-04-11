using System.Diagnostics.CodeAnalysis;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Models;

namespace ArtLink.DataAccess.Converters;

public static class TechniqueConverter
{
    [return: NotNullIfNotNull(nameof(techniqueDb))]
    public static Technique? ToDomain(this TechniqueDb? techniqueDb)
    {
        if (techniqueDb is null)
            return null;

        return new Technique(
            id: techniqueDb.Id,
            name: techniqueDb.Name,
            description: techniqueDb.Description
        );
    }

    [return: NotNullIfNotNull(nameof(technique))]
    public static TechniqueDb? ToDataAccess(this Technique? technique)
    {
        if (technique is null)
            return null;

        return new TechniqueDb(
            id: technique.Id,
            name: technique.Name,
            description: technique.Description
        );
    }
}

