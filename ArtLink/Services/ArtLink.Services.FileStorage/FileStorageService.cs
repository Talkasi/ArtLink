using ArtLink.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ArtLink.Services.FileStorage;

public class FileStorageService(string rootPath) : IFileStorageService
{
    private const string BasePath = "uploads/images";

    public async Task<string> SaveImageAsync(IFormFile file, string folder)
    {
        if (file.Length == 0) throw new ArgumentException("File is empty");

        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!validExtensions.Contains(extension))
            throw new ArgumentException("Invalid image format");

        var fileName = $"{Guid.NewGuid()}{extension}";
        var relativePath = Path.Combine(BasePath, folder, fileName);
        var absolutePath = Path.Combine(rootPath, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using (var stream = new FileStream(absolutePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/{relativePath.Replace('\\', '/')}";
    }

    public Task DeleteImageAsync(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return Task.CompletedTask;

        var absolutePath = Path.Combine(rootPath, relativePath.TrimStart('/'));
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
        return Task.CompletedTask;
    }
}