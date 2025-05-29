using Microsoft.AspNetCore.Http;

namespace ArtLink.Domain.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(IFormFile file, string folder);
    Task DeleteImageAsync(string relativePath);
}