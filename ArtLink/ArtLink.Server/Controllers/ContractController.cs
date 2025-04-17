using System.ComponentModel.DataAnnotations;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Dto.Contract;
using Microsoft.AspNetCore.Mvc;

namespace ArtLink.Server.Controllers;

[ApiController]
[Route("api/contracts")]
public class ContractController(IContractService service, ILogger<ContractController> logger) : ControllerBase
{
    /// <summary>
    /// Создать новый контракт между работодателем и художником.
    /// </summary>
    /// <param name="dto">Данные нового контракта.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody][Required] CreateContractDto dto)
    {
        try
        {
            await service.AddContractAsync(dto.ArtistId, dto.EmployerId, dto.ProjectDescription, dto.Status, startDate: dto.StartDate, endDate: dto.EndDate);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating contract");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получить контракт по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор контракта.</param>
    /// <returns>Информация о контракте.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute][Required] Guid id)
    {
        try
        {
            var contract = await service.GetContractByIdAsync(id);
            if (contract == null) return NotFound();

            return Ok(new ContractDto(contract.Id, contract.ArtistId, contract.EmployerId, contract.ProjectDescription, contract.Status, endDate: contract.EndDate, startDate: contract.StartDate));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetById");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновить существующий контракт.
    /// </summary>
    /// <param name="id">Идентификатор контракта.</param>
    /// <param name="dto">Новые данные контракта.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute][Required] Guid id, [FromBody][Required] CreateContractDto dto)
    {
        try
        {
            await service.UpdateContractAsync(id, dto.ArtistId, dto.EmployerId, dto.ProjectDescription, dto.Status, startDate: dto.StartDate, endDate: dto.EndDate);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating contract");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удалить контракт по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор контракта.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute][Required] Guid id)
    {
        try
        {
            await service.DeleteContractAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting contract");
            return StatusCode(500);
        }
    }
}
