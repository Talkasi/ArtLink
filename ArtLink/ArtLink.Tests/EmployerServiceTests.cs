using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Services.Employer;
using Moq;

namespace ArtLink.Tests;

public class EmployerServiceTests
{
    private readonly Mock<IEmployerRepository> _employerRepositoryMock;
    private readonly EmployerService _employerService;

    public EmployerServiceTests()
    {
        _employerRepositoryMock = new Mock<IEmployerRepository>();
        _employerService = new EmployerService(_employerRepositoryMock.Object);
    }

    [Fact]
    public async Task GetEmployerByIdAsync_ReturnsEmployer_WhenEmployerExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new Employer(id, "Company", "email@example.com", "hash", "John", "Doe");

        _employerRepositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(expected);

        // Act
        var result = await _employerService.GetEmployerByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result!.Id);
    }

    [Fact]
    public async Task GetAllEmployersAsync_ReturnsAllEmployers()
    {
        // Arrange
        var employers = new List<Employer>
        {
            new Employer(Guid.NewGuid(), "Company A", "a@example.com", "hash1", "Anna", "Smith"),
            new Employer(Guid.NewGuid(), "Company B", "b@example.com", "hash2", "Brian", "Jones")
        };

        _employerRepositoryMock.Setup(r => r.GetAllEmployersAsync())
            .ReturnsAsync(employers);

        // Act
        var result = await _employerService.GetAllEmployersAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddEmployerAsync_CallsRepositoryWithCorrectParameters()
    {
        // Arrange
        const string companyName = "Test Corp";
        const string email = "test@corp.com";
        const string cpFirstName = "Jane";
        const string cpLastName = "Doe";

        // Act
        await _employerService.AddEmployerAsync(companyName, email, cpFirstName, cpLastName);

        // Assert
        _employerRepositoryMock.Verify(r => r.AddAsync(
            companyName, email, cpFirstName, cpLastName), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployerAsync_CallsRepositoryWithCorrectParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string companyName = "Updated Corp";
        const string email = "updated@corp.com";
        const string cpFirstName = "Jack";
        const string cpLastName = "White";

        // Act
        await _employerService.UpdateEmployerAsync(id, companyName, email, cpFirstName, cpLastName);

        // Assert
        _employerRepositoryMock.Verify(r => r.UpdateAsync(
            id, companyName, email, cpFirstName, cpLastName), Times.Once);
    }

    [Fact]
    public async Task DeleteEmployerAsync_CallsRepositoryWithCorrectId()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        await _employerService.DeleteEmployerAsync(id);

        // Assert
        _employerRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }
}

