using System.Diagnostics.CodeAnalysis;
using ArtLink.DataAccess.Models;
using ArtLink.Domain.Models;

namespace ArtLink.DataAccess.Converters;

public static class ArtistConverter
{
    [return: NotNullIfNotNull(nameof(artistDb))]
    public static Artist? ToDomain(this ArtistDb? artistDb)
    {
        if (artistDb is null)
            return null;

        return new Artist(
            id: artistDb.Id,
            passwordHash: artistDb.PasswordHash,
            email: artistDb.Email,
            firstName: artistDb.FirstName,
            lastName: artistDb.LastName,
            bio: artistDb.Bio,
            experience: artistDb.Experience,
            profilePicturePath: artistDb.ProfilePicturePath
        );
    }
}

