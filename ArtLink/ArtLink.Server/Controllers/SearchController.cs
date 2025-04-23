using System.ComponentModel.DataAnnotations;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Dto.Artist;
using ArtLink.Dto.ArtWork;
using ArtLink.Dto.Employer;
using Microsoft.AspNetCore.Mvc;

namespace ArtLink.Server.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController(ISearchService service, ILogger<SearchController> logger) : ControllerBase
{
    /// <summary>
    /// Поиск художников по заданной строке.
    /// </summary>
    /// <param name="prompt">Строка поиска (имя, фамилия, email).</param>
    /// <returns>Список найденных художников.</returns>
    [HttpGet("artists")]
    public async Task<IActionResult> SearchArtists([FromQuery][Required] string prompt)
    {
        logger.LogInformation("[SearchController][SearchArtists] Searching artists with prompt: {Prompt}", prompt);

        try
        {
            var result = (await service.SearchArtistsByPromptAsync(prompt)).ToList();
            logger.LogInformation("[SearchController][SearchArtists] Found {Count} artists", result.Count);

            return Ok(result.Select(a => new ArtistDto(a.Id, a.FirstName, a.LastName, a.Email, a.Bio!, a.ProfilePicturePath!, a.Experience ?? 0)).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[SearchController][SearchArtists] Error searching artists with prompt: {Prompt}", prompt);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Поиск работодателей по заданной строке.
    /// </summary>
    /// <param name="prompt">Строка поиска (название компании, имя контактного лица и т.п.).</param>
    /// <returns>Список найденных работодателей.</returns>
    [HttpGet("employers")]
    public async Task<IActionResult> SearchEmployers([FromQuery][Required] string prompt)
    {
        logger.LogInformation("[SearchController][SearchEmployers] Searching employers with prompt: {Prompt}", prompt);

        try
        {
            var result = (await service.SearchEmployersByPromptAsync(prompt)).ToList();
            logger.LogInformation("[SearchController][SearchEmployers] Found {Count} employers", result.Count);

            return Ok(result.Select(e => new EmployerDto(e.Id, e.CompanyName, e.Email, e.CpFirstName, e.CpLastName)).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[SearchController][SearchEmployers] Error searching employers with prompt: {Prompt}", prompt);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Поиск работ по названию или описанию.
    /// </summary>
    /// <param name="prompt">Строка поиска (название или описание работы).</param>
    /// <returns>Список найденных работ.</returns>
    [HttpGet("artworks")]
    public async Task<IActionResult> SearchArtworks([FromQuery][Required] string prompt)
    {
        logger.LogInformation("[SearchController][SearchArtworks] Searching artworks with prompt: {Prompt}", prompt);

        try
        {
            var result = (await service.SearchArtWorksByPromptAsync(prompt)).ToList();
            logger.LogInformation("[SearchController][SearchArtworks] Found {Count} artworks", result.Count);

            return Ok(result.Select(a => new ArtworkDto(a.Id, a.PortfolioId, a.Title, a.ImagePath, a.Description)).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[SearchController][SearchArtworks] Error searching artworks with prompt: {Prompt}", prompt);
            return StatusCode(500);
        }
    }
}
