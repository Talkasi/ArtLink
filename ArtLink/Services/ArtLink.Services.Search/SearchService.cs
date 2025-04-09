using ArtLink.Domain.Models;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Domain.Interfaces.Repositories;

namespace ArtLink.Services.Search;

public class SearchService(
    IArtistRepository artistRepository,
    IEmployerRepository employerRepository,
    IArtworkRepository artworkRepository)
    : ISearchService
{
    public async Task<IEnumerable<Artist>> SearchArtistsByPromptAsync(string prompt)
    {
        return await artistRepository.SearchByPromptAsync(prompt);
    }

    public async Task<IEnumerable<Employer>> SearchEmployersByPromptAsync(string prompt)
    {
        return await employerRepository.SearchByPromptAsync(prompt);
    }

    public async Task<IEnumerable<Artwork>> SearchArtWorksByPromptAsync(string prompt)
    {
        return await artworkRepository.SearchByPromptAsync(prompt);
    }
}
