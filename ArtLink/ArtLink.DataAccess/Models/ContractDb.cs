using System.ComponentModel.DataAnnotations;
using ArtLink.DataAccess.Models.Enums;

namespace ArtLink.DataAccess.Models;

public class ContractDb
{
    public ContractDb(Guid id, 
        Guid artistId, 
        Guid employerId, 
        string projectDescription, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        ContractStateDb status = default)
    {
        Id = id;
        ArtistId = artistId;
        EmployerId = employerId;
        ProjectDescription = projectDescription;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ArtistId { get; set; }

    [Required]
    public Guid EmployerId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string ProjectDescription { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public ContractStateDb Status { get; set; }

    public ArtistDb? Artist { get; set; }
    public EmployerDb? Employer { get; set; }
}
