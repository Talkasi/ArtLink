using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class EmployerDb(
    Guid id,
    string companyName,
    string email,
    string passwordHash,
    string cpFirstName,
    string cpLastName)
{
    [Key]
    public Guid Id { get; init; } = id;

    [Required]
    [MaxLength(255)]
    public string CompanyName { get; set; } = companyName;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = email;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; init; } = passwordHash;

    [Required]
    [MaxLength(100)]
    public string CpFirstName { get; set; } = cpFirstName;

    [Required]
    [MaxLength(100)]
    public string CpLastName { get; set; } = cpLastName;

    public List<ContractDb> Contracts { get; init; } = [];
}
