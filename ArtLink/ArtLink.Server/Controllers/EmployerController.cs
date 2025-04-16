using Microsoft.AspNetCore.Mvc;
using ArtLink.Domain.Interfaces.Services;
using ArtLink.Dto.Employer;
using System.ComponentModel.DataAnnotations;

namespace ArtLink.Server.Controllers;

[ApiController]
[Route("api/employers")]
public class EmployerController(IEmployerService employerService, ILogger<EmployerController> logger) : ControllerBase
{
    /// <summary>
    /// Регистрация нового работодателя.
    /// </summary>
    /// <param name="dto">Данные работодателя для регистрации.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody][Required] RegisterEmployerDto dto)
    {
        try
        {
            await employerService.AddEmployerAsync(dto.CompanyName, dto.Email, dto.PasswordHash, dto.CpFirstName, dto.CpLastName);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in Register employer");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Авторизация работодателя.
    /// </summary>
    /// <param name="dto">Данные для входа работодателя.</param>
    /// <returns>Информация о работодателе при успешной авторизации.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody][Required] LoginEmployerDto dto)
    {
        try
        {
            var employer = await employerService.LoginEmployerAsync(dto.Email, dto.PasswordHash);
            if (employer == null)
                return Unauthorized();

            return Ok(new EmployerDto(employer.Id, employer.CompanyName, employer.Email, employer.CpFirstName, employer.CpLastName));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in Employer login");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Получить информацию о работодателе по ID.
    /// </summary>
    /// <param name="id">Идентификатор работодателя.</param>
    /// <returns>Информация о работодателе.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute][Required] Guid id)
    {
        try
        {
            var employer = await employerService.GetEmployerByIdAsync(id);
            if (employer == null)
                return NotFound();

            return Ok(new EmployerDto(employer.Id, employer.CompanyName, employer.Email, employer.CpFirstName, employer.CpLastName));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetById");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновить данные работодателя.
    /// </summary>
    /// <param name="id">Идентификатор работодателя.</param>
    /// <param name="dto">Новые данные работодателя.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute][Required] Guid id, [FromBody][Required] RegisterEmployerDto dto)
    {
        try
        {
            await employerService.UpdateEmployerAsync(id, dto.CompanyName, dto.Email, dto.CpFirstName, dto.CpLastName);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating employer");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Удалить работодателя по ID.
    /// </summary>
    /// <param name="id">Идентификатор работодателя.</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute][Required] Guid id)
    {
        try
        {
            await employerService.DeleteEmployerAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting employer");
            return StatusCode(500);
        }
    }
}
