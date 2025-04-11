using ArtLink.Domain.Models.Enums;

namespace ArtLink.Domain.Models;

public class Contract(
    Guid id,
    Guid artistId,
    Guid employerId,
    string projectDescription,
    DateTime? startDate,
    DateTime? endDate,
    ContractState status)
{
    public Guid Id { get; set; } = id;

    public Guid ArtistId { get; set; } = artistId;

    public Guid EmployerId { get; set; } = employerId;

    public string ProjectDescription { get; set; } = projectDescription;

    public DateTime? StartDate { get; set; } = startDate;

    public DateTime? EndDate { get; set; } = endDate;

    public ContractState Status { get; set; } = status;
}
