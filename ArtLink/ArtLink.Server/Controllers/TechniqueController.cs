using System.ComponentModel.DataAnnotations;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Dto.Technique;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("[TechniqueController][GetAll] Getting all techniques");

        try
        {
            var techniques = (await techniqueService.GetAllAsync()).ToList();
            logger.LogInformation("[TechniqueController][GetAll] Retrieved {Count} techniques", techniques.Count);

            return Ok(techniques.Select(t => new TechniqueDto(t.Id, t.Name, t.Description)));
        }
        catch (Exception e)
        {
            logger.LogError(e, "[TechniqueController][GetAll] Error retrieving techniques");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Добавить новую технику.
    /// </summary>
    /// <param name="dto">Данные новой техники.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Add([FromBody][Required] CreateTechniqueDto dto)
    {
        logger.LogInformation("[TechniqueController][Add] Adding technique with name: {Name}", dto.Name);

        try
        {
            await techniqueService.AddTechniqueAsync(dto.Name, dto.Description);
            logger.LogInformation("[TechniqueController][Add] Technique added: {Name}", dto.Name);

            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[TechniqueController][Add] Error adding technique: {Name}", dto.Name);
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
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromRoute][Required] Guid id, [FromBody][Required] CreateTechniqueDto dto)
    {
        logger.LogInformation("[TechniqueController][Update] Updating technique {Id} with name: {Name}", id, dto.Name);

        try
        {
            await techniqueService.UpdateTechniqueAsync(id, dto.Name, dto.Description);
            logger.LogInformation("[TechniqueController][Update] Technique updated: {Id}", id);

            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[TechniqueController][Update] Error updating technique: {Id}", id);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удалить технику по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор техники.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete([FromRoute][Required] Guid id)
    {
        logger.LogInformation("[TechniqueController][Delete] Deleting technique {Id}", id);

        try
        {
            await techniqueService.DeleteTechniqueAsync(id);
            logger.LogInformation("[TechniqueController][Delete] Technique deleted: {Id}", id);

            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[TechniqueController][Delete] Error deleting technique: {Id}", id);
            return StatusCode(500);
        }
    }
}
