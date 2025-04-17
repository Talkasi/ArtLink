using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class ArtistDb(
    Guid id,
    string passwordHash,
    string email,
    string firstName,
    string lastName,
    string? bio = null,
    int? experience = null,
    string? profilePicturePath = null)
{
    [Key]
    public Guid Id { get; init; } = id;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; init; } = passwordHash;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = email;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = firstName;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = lastName;

    [MaxLength(1000)]
    public string? Bio { get; set; } = bio;

    public int? Experience { get; set; } = experience;

    [MaxLength(500)]
    public string? ProfilePicturePath { get; set; } = profilePicturePath;

    public List<PortfolioDb> Portfolios { get; init; } = [];

    public List<ContractDb> Contracts { get; init; } = [];
}

