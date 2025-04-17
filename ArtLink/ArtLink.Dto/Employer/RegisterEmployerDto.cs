using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Employer;

public class RegisterEmployerDto(
    string companyName,
    string email,
    string passwordHash,
    string cpFirstName,
    string cpLastName)
{
    [Required]
    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; } = companyName;

    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = email;

    [Required]
    [JsonPropertyName("password_hash")]
    public string PasswordHash { get; set; } = passwordHash;

    [Required]
    [JsonPropertyName("cp_first_name")]
    public string CpFirstName { get; set; } = cpFirstName;

    [Required]
    [JsonPropertyName("cp_last_name")]
    public string CpLastName { get; set; } = cpLastName;
}
