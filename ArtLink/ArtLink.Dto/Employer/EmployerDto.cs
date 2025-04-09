using System;
using System.Text.Json.Serialization;
using ArtLink.Dto.Validator;

namespace ArtLink.Dto.Employer;

public class EmployerDto
{
    public EmployerDto(Guid id, 
        string companyName, 
        string email, 
        string cpFirstName, 
        string cpLastName)
    {
        if (!EmailValidator.IsValidEmail(email))
        {
            throw new ArgumentException("Email is not valid.", nameof(email));
        }

        Id = id;
        CompanyName = companyName;
        Email = email;
        CPFirstName = cpFirstName;
        CPLastName = cpLastName;
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("cp_first_name")]
    public string CPFirstName { get; set; }

    [JsonPropertyName("cp_last_name")]
    public string CPLastName { get; set; }
}
