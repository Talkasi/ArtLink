using System;
using System.Text.Json.Serialization;
using ArtLink.Dto.Validator;

namespace ArtLink.Dto.Employer;

public class LoginEmployerDto
{
    public LoginEmployerDto(string email, 
        string passwordHash)
    {
        if (!EmailValidator.IsValidEmail(email))
        {
            throw new ArgumentException("Email is not valid.", nameof(email));
        }

        Email = email;
        PasswordHash = passwordHash;
    }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; }
}
