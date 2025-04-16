using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Employer;

public class SearchEmployertDto(string searchPrompt)
{
    [Required]
    [JsonPropertyName("search_prompt")]
    public string SearchPrompt { get; set; } = searchPrompt;
}