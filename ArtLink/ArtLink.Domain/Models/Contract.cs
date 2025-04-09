namespace ArtLink.Domain.Models;

public class Contract(
    Guid id,
    Guid artistId,
    Guid employerId,
    string projectDescription,
    DateTime startDate,
    DateTime endDate,
    string status)
{
    public Guid Id { get; set; } = id;

    public Guid ArtistId { get; set; } = artistId;

    public Guid EmployerId { get; set; } = employerId;

    public string ProjectDescription { get; set; } = projectDescription;

    public DateTime StartDate { get; set; } = startDate;

    public DateTime EndDate { get; set; } = endDate;

    public string Status { get; set; } = status;
}
