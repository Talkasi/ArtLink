using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Artist;

public class RegisterArtistDto(
    string firstName,
    string lastName,
    string email,
    string passwordHash,
    string? bio = null,
    string? profilePicturePath = null,
    int? experience = null)
{
    [Required]
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = firstName;

    [Required]
    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = lastName;

    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = email;

    [Required]
    [JsonPropertyName("password_hash")]
    public string PasswordHash { get; set; } = passwordHash;

    [JsonPropertyName("bio")]
    public string? Bio { get; set; } = bio;

    [JsonPropertyName("profile_picture_path")]
    public string? ProfilePicturePath { get; set; } = profilePicturePath;

    [JsonPropertyName("experience")]
    public int? Experience { get; set; } = experience;
}
