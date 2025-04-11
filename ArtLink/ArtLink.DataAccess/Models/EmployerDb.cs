using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class EmployerDb
{
    public EmployerDb(Guid id, 
        string companyName, 
        string email, 
        string passwordHash, 
        string cpFirstName, 
        string cpLastName)
    {
        Id = id;
        CompanyName = companyName;
        Email = email;
        PasswordHash = passwordHash;
        CpFirstName = cpFirstName;
        CpLastName = cpLastName;
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string CompanyName { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; }

    [Required]
    [MaxLength(100)]
    public string CpFirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string CpLastName { get; set; }

    public List<ContractDb> Contracts { get; init; } = [];
}
