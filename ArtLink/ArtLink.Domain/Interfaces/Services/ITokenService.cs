using ArtLink.Domain.Models;
using ArtLink.Domain.Models.Enums;

namespace ArtLink.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(Guid id,
        string email,
        RolesEnum role);
}
