using Moq;
using ArtLink.Domain.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Services.Contract;

namespace ArtLink.Tests;

public class ContractServiceTests
{
    private readonly Mock<IContractRepository> _mockRepo;
    private readonly ContractService _contractService;

    public ContractServiceTests()
    {
        _mockRepo = new Mock<IContractRepository>();
        _contractService = new ContractService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetContractByIdAsync_ShouldReturnContract_WhenContractExists()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var expectedContract = new Contract(contractId, Guid.NewGuid(), Guid.NewGuid(), "Test Project", DateTime.Today, DateTime.Today.AddDays(10), "Active");

        _mockRepo.Setup(repo => repo.GetByIdAsync(contractId)).ReturnsAsync(expectedContract);

        // Act
        var result = await _contractService.GetContractByIdAsync(contractId);

        // Assert
        Assert.Equal(expectedContract, result);
    }

    [Fact]
    public async Task GetAllByArtistIdAsync_ShouldReturnContracts()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var contracts = new List<Contract>
        {
            new Contract(Guid.NewGuid(), artistId, Guid.NewGuid(), "Project A", DateTime.Today, DateTime.Today.AddDays(1), "Pending"),
            new Contract(Guid.NewGuid(), artistId, Guid.NewGuid(), "Project B", DateTime.Today, DateTime.Today.AddDays(2), "Completed")
        };

        _mockRepo.Setup(repo => repo.GetAllByArtistIdAsync(artistId)).ReturnsAsync(contracts);

        // Act
        var result = await _contractService.GetAllByArtistIdAsync(artistId);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllByEmployerIdAsync_ShouldReturnContracts()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var contracts = new List<Contract>
        {
            new Contract(Guid.NewGuid(), Guid.NewGuid(), employerId, "Project A", DateTime.Today, DateTime.Today.AddDays(1), "Pending")
        };

        _mockRepo.Setup(repo => repo.GetAllByEmployerIdAsync(employerId)).ReturnsAsync(contracts);

        // Act
        var result = await _contractService.GetAllByEmployerIdAsync(employerId);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task AddContractAsync_ShouldCallRepository()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var employerId = Guid.NewGuid();
        var description = "Test Project";
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);
        var status = "New";

        _mockRepo.Setup(r => r.AddAsync(artistId, employerId, description, startDate, endDate, status))
                 .Returns(Task.CompletedTask);

        // Act
        await _contractService.AddContractAsync(artistId, employerId, description, startDate, endDate, status);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(artistId, employerId, description, startDate, endDate, status), Times.Once);
    }

    [Fact]
    public async Task UpdateContractAsync_ShouldCallRepository()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var artistId = Guid.NewGuid();
        var employerId = Guid.NewGuid();
        const string description = "Updated Project";
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(10);
        const string status = "Updated";

        _mockRepo.Setup(r => r.UpdateAsync(contractId, artistId, employerId, description, startDate, endDate, status))
                 .Returns(Task.CompletedTask);

        // Act
        await _contractService.UpdateContractAsync(contractId, artistId, employerId, description, startDate, endDate, status);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(contractId, artistId, employerId, description, startDate, endDate, status), Times.Once);
    }

    [Fact]
    public async Task DeleteContractAsync_ShouldCallRepository()
    {
        // Arrange
        var contractId = Guid.NewGuid();

        _mockRepo.Setup(r => r.DeleteAsync(contractId)).Returns(Task.CompletedTask);

        // Act
        await _contractService.DeleteContractAsync(contractId);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(contractId), Times.Once);
    }
}

