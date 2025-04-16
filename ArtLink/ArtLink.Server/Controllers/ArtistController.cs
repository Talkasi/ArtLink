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
        try
        {
            await service.AddArtistAsync(dto.FirstName, dto.LastName, dto.Email, dto.PasswordHash, dto.Bio, dto.ProfilePicturePath, dto.Experience);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error registering artist");
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
        try
        {
            var artist = await service.LoginArtistAsync(dto.Email, dto.PasswordHash);
            if (artist == null) return Unauthorized();

            return Ok(new ArtistDto(artist.Id, artist.FirstName, artist.LastName, artist.Email, artist.Bio, artist.ProfilePicturePath, artist.Experience));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error logging in artist");
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
        try
        {
            var artist = await service.GetArtistByIdAsync(id);
            if (artist == null) return NotFound();

            return Ok(new ArtistDto(artist.Id, artist.FirstName, artist.LastName, artist.Email, artist.Bio, artist.ProfilePicturePath, artist.Experience));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetById");
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
        try
        {
            await service.UpdateArtistAsync(id, dto.FirstName, dto.LastName, dto.Email, dto.Bio, dto.ProfilePicturePath, dto.Experience);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating artist");
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
        try
        {
            await service.DeleteArtistAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting artist");
            return StatusCode(500);
        }
    }
}
