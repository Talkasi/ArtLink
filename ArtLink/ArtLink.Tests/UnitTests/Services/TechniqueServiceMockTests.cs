using Allure.Xunit.Attributes;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Services.Technique;
using ArtLink.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArtLink.Tests.UnitTests.Services;

[AllureSuite("Technique Service Tests")]
[AllureSubSuite("London Style (with Mocks)")]
public class TechniqueServiceMockUnitTests : IClassFixture<TechniqueFixture>
{
    private readonly Mock<ITechniqueRepository> _techniqueRepositoryMock;
    private readonly TechniqueService _sut;
    private readonly TechniqueFixture _fixture;

    public TechniqueServiceMockUnitTests(TechniqueFixture fixture)
    {
        _fixture = fixture;
        _techniqueRepositoryMock = new Mock<ITechniqueRepository>();
        Mock<ILogger<TechniqueService>> loggerMock = new Mock<ILogger<TechniqueService>>();
        _sut = new TechniqueService(_techniqueRepositoryMock.Object, loggerMock.Object);
    }

    [Fact]
    [AllureFeature("GetAllAsync")]
    [AllureStory("Positive case - techniques exist")]
    public async Task GetAllAsync_TechniquesExist_ReturnsTechniques()
    {
        // Arrange
        var techniques = new List<Technique>
        {
            _fixture.CreateTechnique(name: "Digital", description: "Tablet drawing"),
            _fixture.CreateTechnique(name: "Traditional", description: "Oil painting")
        };
        
        _techniqueRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(techniques);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        _techniqueRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllAsync")]
    [AllureStory("Negative case - no techniques")]
    public async Task GetAllAsync_NoTechniques_ReturnsEmptyList()
    {
        // Arrange
        _techniqueRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Technique>());

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
        _techniqueRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    [AllureFeature("GetAllAsync")]
    [AllureStory("Exception handling")]
    public async Task GetAllAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var exception = new Exception("Database error");
        
        _techniqueRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetAllAsync());
        _techniqueRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    [AllureFeature("AddTechniqueAsync")]
    [AllureStory("Positive case - technique added")]
    public async Task AddTechniqueAsync_ValidData_ReturnsId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        
        _techniqueRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddTechniqueAsync("Digital", "Tablet drawing");

        // Assert
        result.Should().Be(expectedId);
        _techniqueRepositoryMock.Verify(repo => repo.AddAsync("Digital", "Tablet drawing"), Times.Once);
    }

    [Fact]
    [AllureFeature("AddTechniqueAsync")]
    [AllureStory("Edge case - empty name")]
    public async Task AddTechniqueAsync_EmptyName_ReturnsId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        
        _techniqueRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.AddTechniqueAsync("", "Description");

        // Assert
        result.Should().Be(expectedId);
        _techniqueRepositoryMock.Verify(repo => repo.AddAsync("", "Description"), Times.Once);
    }

    [Fact]
    [AllureFeature("AddTechniqueAsync")]
    [AllureStory("Exception handling")]
    public async Task AddTechniqueAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var exception = new Exception("Add failed");
        
        _techniqueRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.AddTechniqueAsync("Digital", "Tablet drawing"));
    }

    [Fact]
    [AllureFeature("UpdateTechniqueAsync")]
    [AllureStory("Positive case - technique updated")]
    public async Task UpdateTechniqueAsync_ValidData_UpdatesSuccessfully()
    {
        // Arrange
        var techniqueId = Guid.NewGuid();
        
        _techniqueRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateTechniqueAsync(techniqueId, "Updated Digital", "Updated description");

        // Assert
        _techniqueRepositoryMock.Verify(repo => repo.UpdateAsync(
            techniqueId, "Updated Digital", "Updated description"), Times.Once);
    }

    [Fact]
    [AllureFeature("UpdateTechniqueAsync")]
    [AllureStory("Exception handling")]
    public async Task UpdateTechniqueAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var techniqueId = Guid.NewGuid();
        var exception = new Exception("Update failed");
        
        _techniqueRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.UpdateTechniqueAsync(
            techniqueId, "Digital", "Tablet drawing"));
    }

    [Fact]
    [AllureFeature("DeleteTechniqueAsync")]
    [AllureStory("Positive case - technique deleted")]
    public async Task DeleteTechniqueAsync_ValidId_DeletesSuccessfully()
    {
        // Arrange
        var techniqueId = Guid.NewGuid();
        
        _techniqueRepositoryMock
            .Setup(repo => repo.DeleteAsync(techniqueId))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteTechniqueAsync(techniqueId);

        // Assert
        _techniqueRepositoryMock.Verify(repo => repo.DeleteAsync(techniqueId), Times.Once);
    }

    [Fact]
    [AllureFeature("DeleteTechniqueAsync")]
    [AllureStory("Exception handling")]
    public async Task DeleteTechniqueAsync_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var techniqueId = Guid.NewGuid();
        var exception = new Exception("Delete failed");
        
        _techniqueRepositoryMock
            .Setup(repo => repo.DeleteAsync(techniqueId))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.DeleteTechniqueAsync(techniqueId));
    }
}