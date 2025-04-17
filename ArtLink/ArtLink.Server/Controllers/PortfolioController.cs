using Microsoft.AspNetCore.Mvc;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Dto.Portfolio;
using System.ComponentModel.DataAnnotations;

namespace ArtLink.Server.Controllers;

[ApiController]
[Route("api/portfolios")]
public class PortfolioController(IPortfolioService portfolioService, ILogger<PortfolioController> logger) : ControllerBase
{
    /// <summary>
    /// Создать новое портфолио для художника.
    /// </summary>
    /// <param name="dto">Данные для создания портфолио.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody][Required] CreatePortfolioDto dto)
    {
        try
        {
            await portfolioService.AddPortfolioAsync(dto.ArtistId, dto.Title, dto.TechniqueId, dto.Description);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in Create Portfolio");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получить портфолио по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор портфолио.</param>
    /// <returns>Информация о портфолио.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute][Required] Guid id)
    {
        try
        {
            var portfolio = await portfolioService.GetPortfolioByIdAsync(id);
            if (portfolio == null)
                return NotFound();

            return Ok(new PortfolioDto(portfolio.Id, portfolio.ArtistId, portfolio.Title, portfolio.TechniqueId, portfolio.Description));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetById Portfolio");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получить все портфолио по идентификатору художника.
    /// </summary>
    /// <param name="artistId">Идентификатор художника.</param>
    /// <returns>Список портфолио художника.</returns>
    [HttpGet("artist/{artistId:guid}")]
    public async Task<IActionResult> GetByArtist([FromRoute][Required] Guid artistId)
    {
        try
        {
            var list = await portfolioService.GetAllByArtistIdAsync(artistId);
            return Ok(list.Select(p => new PortfolioDto(p.Id, p.ArtistId, p.Title, p.TechniqueId, p.Description)).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetByArtist");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновить существующее портфолио.
    /// </summary>
    /// <param name="id">Идентификатор портфолио.</param>
    /// <param name="dto">Новые данные портфолио.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute][Required] Guid id, [FromBody][Required] CreatePortfolioDto dto)
    {
        try
        {
            await portfolioService.UpdatePortfolioAsync(id, dto.ArtistId, dto.Title, dto.TechniqueId, dto.Description);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating portfolio");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удалить портфолио по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор портфолио.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute][Required] Guid id)
    {
        try
        {
            await portfolioService.DeletePortfolioAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting portfolio");
            return StatusCode(500);
        }
    }
}