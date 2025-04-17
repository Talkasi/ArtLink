using System.ComponentModel.DataAnnotations;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Dto.Artist;
using Microsoft.AspNetCore.Mvc;

namespace ArtLink.Server.Controllers;

[ApiController]
[Route("api/artists")]
public class ArtistController(IArtistService service, ILogger<ArtistController> logger) : ControllerBase
{
    /// <summary>
    /// Регистрация нового художника.
    /// </summary>
    /// <param name="dto">Данные регистрации художника.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody][Required] RegisterArtistDto dto)
    {
        logger.LogInformation("[ArtistController][Register] Attempting to register artist with email: {Email}", dto.Email);

        try
        {
            await service.AddArtistAsync(dto.FirstName, dto.LastName, dto.Email, dto.PasswordHash, dto.Bio, dto.ProfilePicturePath, dto.Experience);
            logger.LogInformation("[ArtistController][Register] Successfully registered artist: {Email}", dto.Email);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistController][Register] Error registering artist: {Email}", dto.Email);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Авторизация художника.
    /// </summary>
    /// <param name="dto">Данные для входа.</param>
    /// <returns>Информация о художнике при успешной авторизации.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody][Required] LoginArtistDto dto)
    {
        logger.LogInformation("[ArtistController][Login] Attempting login for artist with email: {Email}", dto.Email);

        try
        {
            var artist = await service.LoginArtistAsync(dto.Email, dto.PasswordHash);
            if (artist == null)
            {
                logger.LogWarning("[ArtistController][Login] Login failed for artist with email: {Email}", dto.Email);
                return Unauthorized();
            }

            logger.LogInformation("[ArtistController][Login] Login successful for artist: {Email}", dto.Email);
            return Ok(new ArtistDto(artist.Id, artist.FirstName, artist.LastName, artist.Email, artist.Bio, artist.ProfilePicturePath, artist.Experience));
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistController][Login] Error logging in artist: {Email}", dto.Email);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получить информацию о художнике по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор художника.</param>
    /// <returns>Информация о художнике.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute][Required] Guid id)
    {
        logger.LogInformation("[ArtistController][GetById] Request to get artist with ID: {ArtistId}", id);

        try
        {
            var artist = await service.GetArtistByIdAsync(id);
            if (artist == null)
            {
                logger.LogWarning("[ArtistController][GetById] Artist not found: {ArtistId}", id);
                return NotFound();
            }

            logger.LogInformation("[ArtistController][GetById] Artist found: {ArtistId}", id);
            return Ok(new ArtistDto(artist.Id, artist.FirstName, artist.LastName, artist.Email, artist.Bio, artist.ProfilePicturePath, artist.Experience));
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistController][GetById] Error retrieving artist: {ArtistId}", id);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновить информацию о художнике.
    /// </summary>
    /// <param name="id">Идентификатор художника.</param>
    /// <param name="dto">Новые данные художника.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute][Required] Guid id, [FromBody][Required] RegisterArtistDto dto)
    {
        logger.LogInformation("[ArtistController][Update] Attempting to update artist with ID: {ArtistId}", id);

        try
        {
            await service.UpdateArtistAsync(id, dto.FirstName, dto.LastName, dto.Email, dto.Bio, dto.ProfilePicturePath, dto.Experience);
            logger.LogInformation("[ArtistController][Update] Successfully updated artist with ID: {ArtistId}", id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistController][Update] Error updating artist: {ArtistId}", id);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удалить аккаунт художника.
    /// </summary>
    /// <param name="id">Идентификатор художника.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute][Required] Guid id)
    {
        logger.LogInformation("[ArtistController][Delete] Attempting to delete artist with ID: {ArtistId}", id);

        try
        {
            await service.DeleteArtistAsync(id);
            logger.LogInformation("[ArtistController][Delete] Successfully deleted artist with ID: {ArtistId}", id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtistController][Delete] Error deleting artist: {ArtistId}", id);
            return StatusCode(500);
        }
    }
}
