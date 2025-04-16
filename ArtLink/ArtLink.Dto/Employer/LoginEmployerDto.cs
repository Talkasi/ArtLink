using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Employer;

public class LoginEmployerDto(
    string email,
    string passwordHash)
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = email;

    [Required]
    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; } = passwordHash;
}
