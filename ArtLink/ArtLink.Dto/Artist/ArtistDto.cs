using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Artist;

public class ArtistDto(
    Guid id,
    string firstName,
    string lastName,
    string email,
    string? bio = null,
    string? profilePicturePath = null,
    int? experience = null)
{
    [Required]
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = id;

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

    [JsonPropertyName("bio")]
    public string? Bio { get; set; } = bio;

    [JsonPropertyName("profile_picture_path")]
    public string? ProfilePicturePath { get; set; } = profilePicturePath;

    [JsonPropertyName("experience")]
    public int? Experience { get; set; } = experience;
}

public class ArtistUpdateRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Bio { get; set; }
    public int? Experience { get; set; }
}
