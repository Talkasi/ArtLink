using System.ComponentModel.DataAnnotations;
using ArtLink.DataAccess.Models.Enums;

namespace ArtLink.DataAccess.Models;

public class ContractDb(
    Guid id,
    Guid artistId,
    Guid employerId,
    string projectDescription,
    DateTime? startDate = null,
    DateTime? endDate = null,
    ContractStateDb status = default)
{
    [Key]
    public Guid Id { get; init; } = id;

    [Required]
    public Guid ArtistId { get; set; } = artistId;

    [Required]
    public Guid EmployerId { get; set; } = employerId;

    [Required]
    [MaxLength(2000)]
    public string ProjectDescription { get; set; } = projectDescription;

    public DateTime? StartDate { get; set; } = startDate;

    public DateTime? EndDate { get; set; } = endDate;

    [Required]
    public ContractStateDb Status { get; set; } = status;

    public ArtistDb? Artist { get; init; }
    public EmployerDb? Employer { get; init; }
}
