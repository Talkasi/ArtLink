namespace ArtLink.Domain.Models;

public class Artist(
    Guid id,
    string? passwordHash,
    string email,
    string firstName,
    string lastName,
    string? bio,
    int? experience,
    string? profilePicturePath)
{
    public Guid Id { get; set; } = id;

    public string? PasswordHash { get; set; } = passwordHash;

    public string Email { get; set; } = email;

    public string FirstName { get; set; } = firstName;

    public string LastName { get; set; } = lastName;

    public string? Bio { get; set; } = bio;

    public int? Experience { get; set; } = experience;

    public string? ProfilePicturePath { get; set; } = profilePicturePath;
}
