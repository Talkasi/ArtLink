using System.ComponentModel.DataAnnotations;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Dto.ArtWork;
using Microsoft.AspNetCore.Mvc;

namespace ArtLink.Server.Controllers;

[ApiController]
[Route("api/artworks")]
public class ArtworkController(IArtworkService service, ILogger<ArtworkController> logger) : ControllerBase
{
    /// <summary>
    /// Создать новую работу в рамках портфолио.
    /// </summary>
    /// <param name="dto">Данные о создаваемой работе.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody][Required] ArtworkDto dto)
    {
        try
        {
            await service.AddArtworkAsync(dto.PortfolioId, dto.Title, dto.ImagePath, dto.Description);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating artwork");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получить работу по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор работы.</param>
    /// <returns>Информация о работе.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute][Required] Guid id)
    {
        try
        {
            var artwork = await service.GetArtworkByIdAsync(id);
            if (artwork == null) return NotFound();

            return Ok(new ArtworkDto(artwork.Id, artwork.PortfolioId, artwork.Title, artwork.ImagePath, artwork.Description));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetById");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получить все работы по идентификатору портфолио.
    /// </summary>
    /// <param name="portfolioId">Идентификатор портфолио.</param>
    /// <returns>Список работ из портфолио.</returns>
    [HttpGet("portfolio/{portfolioId:guid}")]
    public async Task<IActionResult> GetByPortfolio([FromRoute][Required] Guid portfolioId)
    {
        try
        {
            var list = await service.GetAllByPortfolioIdAsync(portfolioId);
            return Ok(list.Select(a => new ArtworkDto(a.Id, a.PortfolioId, a.Title, a.ImagePath, a.Description)).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetByPortfolio");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновить информацию о работе.
    /// </summary>
    /// <param name="id">Идентификатор работы.</param>
    /// <param name="dto">Новые данные работы.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute][Required] Guid id, [FromBody][Required] CreateArtworkDto dto)
    {
        try
        {
            await service.UpdateArtworkAsync(id, dto.PortfolioId, dto.Title, dto.ImagePath, dto.Description);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating artwork");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удалить работу по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор работы.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute][Required] Guid id)
    {
        try
        {
            await service.DeleteArtworkAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting artwork");
            return StatusCode(500);
        }
    }
}
