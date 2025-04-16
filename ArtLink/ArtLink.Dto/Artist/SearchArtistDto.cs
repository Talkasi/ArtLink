using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArtLink.Dto.Artist;

public class SearchArtistDto(string searchPrompt, int? experience = null)
{
    [Required]
    [JsonPropertyName("search_prompt")]
    public string SearchPrompt { get; set; } = searchPrompt;

    [JsonPropertyName("experience")]
    public int? Experience { get; set; } = experience;
}