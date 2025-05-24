using Moq;
using ArtLink.Domain.Models;
using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models.Enums;
using ArtLink.Services.Contract;
using Microsoft.Extensions.Logging;

namespace ArtLink.Tests.Services;

public class ContractServiceTests
{
    private readonly Mock<IContractRepository> _mockRepo;
    private readonly ContractService _contractService;

    public ContractServiceTests()
    {
        _mockRepo = new Mock<IContractRepository>();
        var loggerMock =  new Mock<ILogger<ContractService>>();
        _contractService = new ContractService(_mockRepo.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetContractByIdAsync_ShouldReturnContract_WhenContractExists()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var expectedContract = new Contract(contractId, Guid.NewGuid(), Guid.NewGuid(), "Test Project", DateTime.Today, DateTime.Today.AddDays(10), ContractState.Accepted);

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
            new Contract(Guid.NewGuid(), artistId, Guid.NewGuid(), "Project A", DateTime.Today, DateTime.Today.AddDays(1), ContractState.Draft),
            new Contract(Guid.NewGuid(), artistId, Guid.NewGuid(), "Project B", DateTime.Today, DateTime.Today.AddDays(2), ContractState.Send)
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
            new Contract(Guid.NewGuid(), Guid.NewGuid(), employerId, "Project A", DateTime.Today, DateTime.Today.AddDays(1), ContractState.Rejected)
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
        const string description = "Test Project";
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);
        const ContractState status = ContractState.Draft;

        _mockRepo.Setup(r => r.AddAsync(artistId, employerId, description, status, startDate, endDate));

        // Act
        await _contractService.AddContractAsync(artistId, employerId, description, status, startDate: startDate, endDate: endDate);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(artistId, employerId, description, status, startDate, endDate), Times.Once);
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
        const ContractState status = ContractState.Accepted;

        _mockRepo.Setup(r => r.UpdateAsync(contractId, artistId, employerId, description, status, startDate, endDate))
                 .Returns(Task.CompletedTask);

        // Act
        await _contractService.UpdateContractAsync(contractId, artistId, employerId, description, status, startDate: startDate, endDate: endDate);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(contractId, artistId, employerId, description, status, startDate, endDate), Times.Once);
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

