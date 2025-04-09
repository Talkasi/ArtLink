using System;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Contract;

public class CreateContractDto(
    Guid artistId,
    Guid employerId,
    string projectDescription,
    DateTime startDate,
    DateTime endDate,
    string status)
{
    [JsonPropertyName("artist_id")]
    public Guid ArtistId { get; set; } = artistId;

    [JsonPropertyName("employer_id")]
    public Guid EmployerId { get; set; } = employerId;

    [JsonPropertyName("project_description")]
    public string ProjectDescription { get; set; } = projectDescription;

    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; } = startDate;

    [JsonPropertyName("end_date")]
    public DateTime EndDate { get; set; } = endDate;

    [JsonPropertyName("status")]
    public string Status { get; set; } = status;
}
