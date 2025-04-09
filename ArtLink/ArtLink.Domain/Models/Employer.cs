namespace ArtLink.Domain.Models;

public class Employer(
    Guid id,
    string companyName,
    string email,
    string passwordHash,
    string cpFirstName,
    string cpLastName)
{
    public Guid Id { get; set; } = id;

    public string CompanyName { get; set; } = companyName;

    public string Email { get; set; } = email;

    public string PasswordHash { get; set; } = passwordHash;

    public string CPFirstName { get; set; } = cpFirstName;

    public string CPLastName { get; set; } = cpLastName;
}
