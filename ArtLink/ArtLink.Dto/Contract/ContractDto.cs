using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ArtLink.Domain.Models.Enums;

namespace ArtLink.Dto.Contract;
public class ContractDto(
    Guid id,
    Guid artistId,
    Guid employerId,
    string projectDescription,
    ContractState status,
    DateTime? endDate = null,
    DateTime? startDate = null)
{
    [Required]
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = id;

    [Required]
    [JsonPropertyName("artist_id")]
    public Guid ArtistId { get; set; } = artistId;

    [Required]
    [JsonPropertyName("employer_id")]
    public Guid EmployerId { get; set; } = employerId;

    [Required]
    [JsonPropertyName("project_description")]
    public string ProjectDescription { get; set; } = projectDescription;

    [JsonPropertyName("start_date")]
    public DateTime? StartDate { get; set; } = startDate;

    [JsonPropertyName("end_date")]
    public DateTime? EndDate { get; set; } = endDate;

    [Required]
    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContractState Status { get; set; } = status;
}
