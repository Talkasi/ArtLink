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
        logger.LogInformation("[PortfolioController][Create] Creating portfolio for artist: {ArtistId}", dto.ArtistId);

        try
        {
            await portfolioService.AddPortfolioAsync(dto.ArtistId, dto.Title, dto.TechniqueId, dto.Description);
            logger.LogInformation("[PortfolioController][Create] Portfolio created successfully for artist: {ArtistId}", dto.ArtistId);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioController][Create] Error creating portfolio for artist: {ArtistId}", dto.ArtistId);
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
        logger.LogInformation("[PortfolioController][GetById] Fetching portfolio with ID: {PortfolioId}", id);

        try
        {
            var portfolio = await portfolioService.GetPortfolioByIdAsync(id);
            if (portfolio == null)
            {
                logger.LogWarning("[PortfolioController][GetById] Portfolio not found: {PortfolioId}", id);
                return NotFound();
            }

            logger.LogInformation("[PortfolioController][GetById] Portfolio retrieved: {PortfolioId}", id);
            return Ok(new PortfolioDto(portfolio.Id, portfolio.ArtistId, portfolio.Title, portfolio.TechniqueId, portfolio.Description));
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioController][GetById] Error retrieving portfolio: {PortfolioId}", id);
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
        logger.LogInformation("[PortfolioController][GetByArtist] Fetching portfolios for artist: {ArtistId}", artistId);

        try
        {
            var list = (await portfolioService.GetAllByArtistIdAsync(artistId)).ToList();
            logger.LogInformation("[PortfolioController][GetByArtist] Retrieved {Count} portfolios for artist: {ArtistId}", list.Count, artistId);

            return Ok(list.Select(p => new PortfolioDto(p.Id, p.ArtistId, p.Title, p.TechniqueId, p.Description)).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioController][GetByArtist] Error retrieving portfolios for artist: {ArtistId}", artistId);
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
        logger.LogInformation("[PortfolioController][Update] Updating portfolio: {PortfolioId}", id);

        try
        {
            await portfolioService.UpdatePortfolioAsync(id, dto.ArtistId, dto.Title, dto.TechniqueId, dto.Description);
            logger.LogInformation("[PortfolioController][Update] Portfolio updated successfully: {PortfolioId}", id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioController][Update] Error updating portfolio: {PortfolioId}", id);
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
        logger.LogInformation("[PortfolioController][Delete] Deleting portfolio: {PortfolioId}", id);

        try
        {
            await portfolioService.DeletePortfolioAsync(id);
            logger.LogInformation("[PortfolioController][Delete] Portfolio deleted successfully: {PortfolioId}", id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[PortfolioController][Delete] Error deleting portfolio: {PortfolioId}", id);
            return StatusCode(500);
        }
    }
}
