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
        logger.LogInformation("[EmployerController][Register] Registering employer with email: {Email}", dto.Email);

        try
        {
            await employerService.AddEmployerAsync(dto.CompanyName, dto.Email, dto.PasswordHash, dto.CpFirstName, dto.CpLastName);
            logger.LogInformation("[EmployerController][Register] Successfully registered employer with email: {Email}", dto.Email);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerController][Register] Error registering employer with email: {Email}", dto.Email);
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
        logger.LogInformation("[EmployerController][Login] Logging in employer with email: {Email}", dto.Email);

        try
        {
            var employer = await employerService.LoginEmployerAsync(dto.Email, dto.PasswordHash);
            if (employer == null)
            {
                logger.LogWarning("[EmployerController][Login] Unauthorized login attempt for email: {Email}", dto.Email);
                return Unauthorized();
            }

            logger.LogInformation("[EmployerController][Login] Successful login for employer ID: {EmployerId}", employer.Id);
            return Ok(new EmployerDto(employer.Id, employer.CompanyName, employer.Email, employer.CpFirstName, employer.CpLastName));
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerController][Login] Error during login for email: {Email}", dto.Email);
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
        logger.LogInformation("[EmployerController][GetById] Fetching employer with ID: {EmployerId}", id);

        try
        {
            var employer = await employerService.GetEmployerByIdAsync(id);
            if (employer == null)
            {
                logger.LogWarning("[EmployerController][GetById] Employer not found: {EmployerId}", id);
                return NotFound();
            }

            logger.LogInformation("[EmployerController][GetById] Employer found: {EmployerId}", employer.Id);
            return Ok(new EmployerDto(employer.Id, employer.CompanyName, employer.Email, employer.CpFirstName, employer.CpLastName));
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerController][GetById] Error fetching employer: {EmployerId}", id);
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
        logger.LogInformation("[EmployerController][Update] Updating employer: {EmployerId}", id);

        try
        {
            await employerService.UpdateEmployerAsync(id, dto.CompanyName, dto.Email, dto.CpFirstName, dto.CpLastName);
            logger.LogInformation("[EmployerController][Update] Successfully updated employer: {EmployerId}", id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerController][Update] Error updating employer: {EmployerId}", id);
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
        logger.LogInformation("[EmployerController][Delete] Deleting employer: {EmployerId}", id);

        try
        {
            await employerService.DeleteEmployerAsync(id);
            logger.LogInformation("[EmployerController][Delete] Successfully deleted employer: {EmployerId}", id);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[EmployerController][Delete] Error deleting employer: {EmployerId}", id);
            return StatusCode(500);
        }
    }
}
