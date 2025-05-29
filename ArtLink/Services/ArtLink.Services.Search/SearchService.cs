using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ArtLink.Services.Search;

public class SearchService(
    IArtistRepository artistRepository,
    IEmployerRepository employerRepository,
    IArtworkRepository artworkRepository,
    ILogger<SearchService> logger)
    : ISearchService
{
    public async Task<IEnumerable<Artist>> SearchArtistsByPromptAsync(string prompt)
    {
        try
        {
            logger.LogInformation("[SearchService][SearchArtistsByPrompt] Searching for artists with prompt: {Prompt}", prompt);
            var artists = (await artistRepository.SearchByPromptAsync(prompt)).ToList();
            logger.LogInformation("[SearchService][SearchArtistsByPrompt] Found {Count} artists", artists.Count);
            return artists;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[SearchService][SearchArtistsByPrompt] Error searching artists with prompt: {Prompt}", prompt);
            throw;
        }
    }

    public async Task<IEnumerable<Employer>> SearchEmployersByPromptAsync(string prompt)
    {
        try
        {
            logger.LogInformation("[SearchService][SearchEmployersByPrompt] Searching for employers with prompt: {Prompt}", prompt);
            var employers = (await employerRepository.SearchByPromptAsync(prompt)).ToList();
            logger.LogInformation("[SearchService][SearchEmployersByPrompt] Found {Count} employers", employers.Count);
            return employers;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[SearchService][SearchEmployersByPrompt] Error searching employers with prompt: {Prompt}", prompt);
            throw;
        }
    }

    public async Task<IEnumerable<Artwork>> SearchArtWorksByPromptAsync(string prompt)
    {
        try
        {
            logger.LogInformation("[SearchService][SearchArtWorksByPrompt] Searching for artworks with prompt: {Prompt}", prompt);
            var artworks = (await artworkRepository.SearchByPromptAsync(prompt)).ToList();
            logger.LogInformation("[SearchService][SearchArtWorksByPrompt] Found {Count} artworks", artworks.Count);
            return artworks;
        }
        catch (Exception e)
        {
            logger.LogError(e, "[SearchService][SearchArtWorksByPrompt] Error searching artworks with prompt: {Prompt}", prompt);
            throw;
        }
    }
}
