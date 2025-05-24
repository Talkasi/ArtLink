namespace ArtLink.Domain.Models.Enums;

public enum RolesEnum
{
    Admin = 0,
    Artist = 1,
    Employer = 2
}

public static class Roles
{
    public readonly static string[] RoleNames =
    [
        "Admin",
        "Artist",
        "Employer"
    ];
}

