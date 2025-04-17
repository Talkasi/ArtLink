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
        logger.LogInformation("[ArtworkController][Create] Creating artwork in portfolio {PortfolioId} with title: {Title}", dto.PortfolioId, dto.Title);

        try
        {
            await service.AddArtworkAsync(dto.PortfolioId, dto.Title, dto.ImagePath, dto.Description);
            logger.LogInformation("[ArtworkController][Create] Artwork successfully created in portfolio {PortfolioId}", dto.PortfolioId);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkController][Create] Error creating artwork in portfolio {PortfolioId}", dto.PortfolioId);
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
        logger.LogInformation("[ArtworkController][GetById] Retrieving artwork with ID: {ArtworkId}", id);

        try
        {
            var artwork = await service.GetArtworkByIdAsync(id);
            if (artwork == null)
            {
                logger.LogWarning("[ArtworkController][GetById] Artwork not found: {ArtworkId}", id);
                return NotFound();
            }

            logger.LogInformation("[ArtworkController][GetById] Artwork found: {ArtworkId}", id);
            return Ok(new ArtworkDto(artwork.Id, artwork.PortfolioId, artwork.Title, artwork.ImagePath, artwork.Description));
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkController][GetById] Error retrieving artwork: {ArtworkId}", id);
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
        logger.LogInformation("[ArtworkController][GetByPortfolio] Getting artworks for portfolio: {PortfolioId}", portfolioId);

        try
        {
            var list = (await service.GetAllByPortfolioIdAsync(portfolioId)).ToList();
            logger.LogInformation("[ArtworkController][GetByPortfolio] Retrieved {Count} artworks for portfolio: {PortfolioId}", list.Count(), portfolioId);
            return Ok(list.Select(a => new ArtworkDto(a.Id, a.PortfolioId, a.Title, a.ImagePath, a.Description)).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkController][GetByPortfolio] Error retrieving artworks for portfolio: {PortfolioId}", portfolioId);
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
        logger.LogInformation("[ArtworkController][Update] Updating artwork: {ArtworkId}", id);

        try
        {
            await service.UpdateArtworkAsync(id, dto.PortfolioId, dto.Title, dto.ImagePath, dto.Description);
            logger.LogInformation("[ArtworkController][Update] Successfully updated artwork: {ArtworkId}", id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkController][Update] Error updating artwork: {ArtworkId}", id);
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
        logger.LogInformation("[ArtworkController][Delete] Deleting artwork: {ArtworkId}", id);

        try
        {
            await service.DeleteArtworkAsync(id);
            logger.LogInformation("[ArtworkController][Delete] Successfully deleted artwork: {ArtworkId}", id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[ArtworkController][Delete] Error deleting artwork: {ArtworkId}", id);
            return StatusCode(500);
        }
    }
}
