using System.ComponentModel.DataAnnotations;

namespace ArtLink.DataAccess.Models;

public class ArtistDb
{
    public ArtistDb(Guid id, 
        string passwordHash, 
        string email, 
        string firstName, 
        string lastName, 
        string? bio = null, 
        int? experience = null, 
        string? profilePicturePath = null)
    {
        Id = id;
        PasswordHash = passwordHash;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        Experience = experience;
        ProfilePicturePath = profilePicturePath;
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public int? Experience { get; set; }

    [MaxLength(500)]
    public string? ProfilePicturePath { get; set; }

    public List<PortfolioDb> Portfolios { get; init; } = [];
    public List<ContractDb> Contracts { get; init; } = [];
}

