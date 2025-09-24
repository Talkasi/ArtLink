using Allure.Xunit.Attributes;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Services.Employer;
using ArtLink.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArtLink.Tests.UnitTests.Services;

[AllureSuite("Employer Service Tests")]
[AllureSubSuite("London Style (with Mocks)")]
public class EmployerServiceMockUnitTests : IClassFixture<EmployerFixture>
{
    private readonly Mock<IEmployerRepository> _repoMock;
    private readonly EmployerService _sut;
    private readonly EmployerFixture _fixture;

    public EmployerServiceMockUnitTests(EmployerFixture fixture)
    {
        _fixture = fixture;
        _repoMock = new Mock<IEmployerRepository>();
        Mock<ILogger<EmployerService>> loggerMock = new Mock<ILogger<EmployerService>>();
        _sut = new EmployerService(_repoMock.Object, loggerMock.Object);
    }

    // ----- GetEmployerByIdAsync -----

    [Fact]
    [AllureFeature("GetEmployerByIdAsync")]
    [AllureStory("Positive case - employer exists")]
    public async Task GetEmployerByIdAsync_WhenEmployerExists_ReturnsEmployer()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = _fixture.CreateEmployer(
            id: id,
            companyName: "Special Company",
            email: "special@company.com");

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(expected);

        // Act
        var result = await _sut.GetEmployerByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expected);
        result.CompanyName.Should().Be("Special Company");
        result.Email.Should().Be("special@company.com");
        _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    [AllureFeature("GetEmployerByIdAsync")]
    [AllureStory("Negative case - employer not found")]
    public async Task GetEmployerByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Employer?)null);

        // Act
        var result = await _sut.GetEmployerByIdAsync(id);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    [AllureFeature("GetEmployerByIdAsync")]
    [AllureStory("Exception handling")]
    public async Task GetEmployerByIdAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var exception = new Exception("DB error");

        _repoMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetEmployerByIdAsync(id));
        _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    // ----- GetAllEmployersAsync -----

    [Fact]
    [AllureFeature("GetAllEmployersAsync")]
    [AllureStory("Positive case - returns employers")]
    public async Task GetAllEmployersAsync_WhenThereAreEmployers_ReturnsList()
    {
        // Arrange
        var employers = _fixture.CreateEmployers(3);

        _repoMock.Setup(r => r.GetAllEmployersAsync()).ReturnsAsync(employers);

        // Act
        var result = await _sut.GetAllEmployersAsync();

        // Assert
        var enumerable = result as Employer[] ?? result.ToArray();
        enumerable.Should().HaveCount(3);
        enumerable.Select(e => e.CompanyName).Should().Contain(["Company 1", "Company 2", "Company 3"]);
        _repoMock.Verify(r => r.GetAllEmployersAsync(), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllEmployersAsync")]
    [AllureStory("Negative case - empty list")]
    public async Task GetAllEmployersAsync_WhenNoEmployers_ReturnsEmpty()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllEmployersAsync()).ReturnsAsync(new List<Employer>());

        // Act
        var result = await _sut.GetAllEmployersAsync();

        // Assert
        result.Should().BeEmpty();
        _repoMock.Verify(r => r.GetAllEmployersAsync(), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllEmployersAsync")]
    [AllureStory("With specific employers")]
    public async Task GetAllEmployersAsync_WithMixedEmployers_ReturnsAll()
    {
        // Arrange
        var mixedEmployers = new List<Employer>
        {
            _fixture.CreateEmployer(companyName: "Tech Corp", email: "tech@corp.com"),
            _fixture.CreateEmployer(companyName: "Design Studio", email: "hello@design.com"),
            _fixture.CreateEmployer(companyName: "Art Gallery", email: "info@gallery.com")
        };

        _repoMock.Setup(r => r.GetAllEmployersAsync()).ReturnsAsync(mixedEmployers);

        // Act
        var result = await _sut.GetAllEmployersAsync();

        // Assert
        var enumerable = result as Employer[] ?? result.ToArray();
        enumerable.Should().HaveCount(3);
        enumerable.Select(e => e.CompanyName).Should().Contain(["Tech Corp", "Design Studio", "Art Gallery"]);
        _repoMock.Verify(r => r.GetAllEmployersAsync(), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllEmployersAsync")]
    [AllureStory("Exception handling")]
    public async Task GetAllEmployersAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var exception = new Exception("DB fail");
        _repoMock.Setup(r => r.GetAllEmployersAsync()).ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetAllEmployersAsync());
        _repoMock.Verify(r => r.GetAllEmployersAsync(), Times.Once);
    }

    // ----- AddEmployerAsync -----

    [Fact]
    [AllureFeature("AddEmployerAsync")]
    [AllureStory("Positive case - returns new id and calls repo")]
    public async Task AddEmployerAsync_ValidData_CallsRepositoryAndReturnsId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var employer = _fixture.CreateEmployer(
            companyName: "NewCo",
            email: "new@co.com",
            cpFirstName: "John",
            cpLastName: "Doe");

        _repoMock.Setup(r => r.AddAsync(
            employer.CompanyName, employer.Email, employer.PasswordHash, 
            employer.CpFirstName, employer.CpLastName))
                 .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddEmployerAsync(
            employer.CompanyName, employer.Email, employer.PasswordHash, 
            employer.CpFirstName, employer.CpLastName);

        // Assert
        result.Should().Be(expectedId);
        _repoMock.Verify(r => r.AddAsync(
            "NewCo", "new@co.com", employer.PasswordHash, "John", "Doe"), Times.Once);
    }

    [Fact]
    [AllureFeature("AddEmployerAsync")]
    [AllureStory("Edge case - empty company name")]
    public async Task AddEmployerAsync_EmptyCompanyName_CallsRepository()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var employer = _fixture.CreateEmployer(companyName: "");

        _repoMock.Setup(r => r.AddAsync("", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                 .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddEmployerAsync(
            "", employer.Email, employer.PasswordHash, 
            employer.CpFirstName, employer.CpLastName);

        // Assert
        result.Should().Be(expectedId);
        _repoMock.Verify(r => r.AddAsync(
            "", employer.Email, employer.PasswordHash, 
            employer.CpFirstName, employer.CpLastName), Times.Once);
    }

    [Fact]
    [AllureFeature("AddEmployerAsync")]
    [AllureStory("With minimal data")]
    public async Task AddEmployerAsync_WithMinimalData_CallsRepository()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var employer = _fixture.CreateEmployer(
            companyName: "Minimal Co",
            cpFirstName: "A",
            cpLastName: "B");

        _repoMock.Setup(r => r.AddAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                 .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddEmployerAsync(
            employer.CompanyName, employer.Email, employer.PasswordHash, 
            employer.CpFirstName, employer.CpLastName);

        // Assert
        result.Should().Be(expectedId);
        _repoMock.Verify(r => r.AddAsync(
            "Minimal Co", employer.Email, employer.PasswordHash, "A", "B"), Times.Once);
    }

    [Fact]
    [AllureFeature("AddEmployerAsync")]
    [AllureStory("Exception handling")]
    public async Task AddEmployerAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var employer = _fixture.CreateEmployer();
        var exception = new Exception("Add error");

        _repoMock.Setup(r => r.AddAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                 .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.AddEmployerAsync(
            employer.CompanyName, employer.Email, employer.PasswordHash, 
            employer.CpFirstName, employer.CpLastName));
        _repoMock.Verify(r => r.AddAsync(
            employer.CompanyName, employer.Email, employer.PasswordHash, 
            employer.CpFirstName, employer.CpLastName), Times.Once);
    }

    // ----- UpdateEmployerAsync -----

    [Fact]
    [AllureFeature("UpdateEmployerAsync")]
    [AllureStory("Positive case - calls repository")]
    public async Task UpdateEmployerAsync_ValidData_CallsRepository()
    {
        // Arrange
        var employer = _fixture.CreateEmployer(
            companyName: "UpdatedCo",
            email: "updated@co.com",
            cpFirstName: "Jane",
            cpLastName: "Smith");

        _repoMock.Setup(r => r.UpdateAsync(
            employer.Id, employer.CompanyName, employer.Email, 
            employer.CpFirstName, employer.CpLastName))
                 .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateEmployerAsync(
            employer.Id, employer.CompanyName, employer.Email, 
            employer.CpFirstName, employer.CpLastName);

        // Assert
        _repoMock.Verify(r => r.UpdateAsync(
            employer.Id, "UpdatedCo", "updated@co.com", "Jane", "Smith"), Times.Once);
    }

    [Fact]
    [AllureFeature("UpdateEmployerAsync")]
    [AllureStory("With fixture employer")]
    public async Task UpdateEmployerAsync_WithFixtureEmployer_CallsRepository()
    {
        // Arrange
        var employer = _fixture.CreateEmployer();

        _repoMock.Setup(r => r.UpdateAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateEmployerAsync(
            employer.Id, employer.CompanyName, employer.Email, 
            employer.CpFirstName, employer.CpLastName);

        // Assert
        _repoMock.Verify(r => r.UpdateAsync(
            employer.Id, employer.CompanyName, employer.Email, 
            employer.CpFirstName, employer.CpLastName), Times.Once);
    }

    [Fact]
    [AllureFeature("UpdateEmployerAsync")]
    [AllureStory("Exception handling")]
    public async Task UpdateEmployerAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var employer = _fixture.CreateEmployer();
        var exception = new Exception("Update failed");

        _repoMock.Setup(r => r.UpdateAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                 .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.UpdateEmployerAsync(
            employer.Id, employer.CompanyName, employer.Email, 
            employer.CpFirstName, employer.CpLastName));
        _repoMock.Verify(r => r.UpdateAsync(
            employer.Id, employer.CompanyName, employer.Email, 
            employer.CpFirstName, employer.CpLastName), Times.Once);
    }

    // ----- DeleteEmployerAsync -----

    [Fact]
    [AllureFeature("DeleteEmployerAsync")]
    [AllureStory("Positive case - calls repository")]
    public async Task DeleteEmployerAsync_ValidId_CallsRepository()
    {
        // Arrange
        var employer = _fixture.CreateEmployer();
        _repoMock.Setup(r => r.DeleteAsync(employer.Id)).Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteEmployerAsync(employer.Id);

        // Assert
        _repoMock.Verify(r => r.DeleteAsync(employer.Id), Times.Once);
    }

    [Fact]
    [AllureFeature("DeleteEmployerAsync")]
    [AllureStory("With specific employer")]
    public async Task DeleteEmployerAsync_WithSpecificEmployer_CallsRepository()
    {
        // Arrange
        var employer = _fixture.CreateEmployer(companyName: "Company to delete");
        _repoMock.Setup(r => r.DeleteAsync(employer.Id)).Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteEmployerAsync(employer.Id);

        // Assert
        _repoMock.Verify(r => r.DeleteAsync(employer.Id), Times.Once);
    }

    [Fact]
    [AllureFeature("DeleteEmployerAsync")]
    [AllureStory("Exception handling")]
    public async Task DeleteEmployerAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var exception = new Exception("Delete failed");

        _repoMock.Setup(r => r.DeleteAsync(employerId)).ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.DeleteEmployerAsync(employerId));
        _repoMock.Verify(r => r.DeleteAsync(employerId), Times.Once);
    }

    // ----- LoginEmployerAsync -----

    [Fact]
    [AllureFeature("LoginEmployerAsync")]
    [AllureStory("Positive case - valid credentials")]
    public async Task LoginEmployerAsync_ValidCredentials_ReturnsEmployer()
    {
        // Arrange
        var expected = _fixture.CreateEmployer(
            email: "ok@e.com", 
            passwordHash: "correct_hash");

        _repoMock.Setup(r => r.LoginAsync("ok@e.com", "correct_hash")).ReturnsAsync(expected);

        // Act
        var result = await _sut.LoginEmployerAsync("ok@e.com", "correct_hash");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expected);
        result.Email.Should().Be("ok@e.com");
        _repoMock.Verify(r => r.LoginAsync("ok@e.com", "correct_hash"), Times.Once);
    }

    [Fact]
    [AllureFeature("LoginEmployerAsync")]
    [AllureStory("Negative case - invalid credentials")]
    public async Task LoginEmployerAsync_InvalidCredentials_ReturnsNull()
    {
        // Arrange
        _repoMock.Setup(r => r.LoginAsync("bad@e.com", "bad")).ReturnsAsync((Employer?)null);

        // Act
        var result = await _sut.LoginEmployerAsync("bad@e.com", "bad");

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.LoginAsync("bad@e.com", "bad"), Times.Once);
    }

    [Fact]
    [AllureFeature("LoginEmployerAsync")]
    [AllureStory("With fixture employer credentials")]
    public async Task LoginEmployerAsync_WithFixtureEmployer_ReturnsEmployer()
    {
        // Arrange
        var employer = _fixture.CreateEmployer();
        _repoMock.Setup(r => r.LoginAsync(employer.Email, employer.PasswordHash)).ReturnsAsync(employer);

        // Act
        var result = await _sut.LoginEmployerAsync(employer.Email, employer.PasswordHash);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(employer);
        _repoMock.Verify(r => r.LoginAsync(employer.Email, employer.PasswordHash), Times.Once);
    }

    [Fact]
    [AllureFeature("LoginEmployerAsync")]
    [AllureStory("Exception handling")]
    public async Task LoginEmployerAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var exception = new Exception("Login failed");
        _repoMock.Setup(r => r.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                 .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.LoginEmployerAsync("a@b", "h"));
        _repoMock.Verify(r => r.LoginAsync("a@b", "h"), Times.Once);
    }
}