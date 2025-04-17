using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Portfolio;

public class SearchPortfolioDto(string searchPrompt, List<Guid>? techniqueIds = null)
{
    [Required]
    [JsonPropertyName("search_prompt")]
    public string SearchPrompt { get; set; } = searchPrompt;

    [JsonPropertyName("technique_ids")]
    public List<Guid>? TechniqueIds { get; set; } = techniqueIds;
}
