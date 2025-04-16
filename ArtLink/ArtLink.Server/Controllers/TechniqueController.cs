using System.ComponentModel.DataAnnotations;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Dto.Technique;
using Microsoft.AspNetCore.Mvc;

namespace ArtLink.Server.Controllers;

[ApiController]
[Route("api/techniques")]
public class TechniqueController(ITechniqueService techniqueService, ILogger<TechniqueController> logger) : ControllerBase
{
    /// <summary>
    /// Получить список всех техник.
    /// </summary>
    /// <returns>Список техник.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var techniques = await techniqueService.GetAllAsync();
            return Ok(techniques.Select(t => new TechniqueDto(t.Id, t.Name, t.Description)));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in {method}", nameof(GetAll));
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Добавить новую технику.
    /// </summary>
    /// <param name="dto">Данные новой техники.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody][Required] CreateTechniqueDto dto)
    {
        try
        {
            await techniqueService.AddTechniqueAsync(dto.Name, dto.Description);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in {method}", nameof(Add));
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновить существующую технику.
    /// </summary>
    /// <param name="id">Идентификатор техники.</param>
    /// <param name="dto">Новые данные техники.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute][Required] Guid id, [FromBody][Required] CreateTechniqueDto dto)
    {
        try
        {
            await techniqueService.UpdateTechniqueAsync(id, dto.Name, dto.Description);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in {method}", nameof(Update));
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удалить технику по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор техники.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute][Required] Guid id)
    {
        try
        {
            await techniqueService.DeleteTechniqueAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in {method}", nameof(Delete));
            return StatusCode(500);
        }
    }
}