using ArtLink.Domain.Interfaces.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Domain.Models.Enums;
using ArtLink.Services.Contract;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArtLink.Tests.UnitTests.Services;

public class ContractServiceUnitTests : IClassFixture<ContractFixture>
{
    private readonly Mock<IContractRepository> _mockRepo;
    private readonly ContractService _contractService;
    private readonly ContractFixture _fixture;

    public ContractServiceUnitTests(ContractFixture fixture)
    {
        _fixture = fixture;
        _mockRepo = new Mock<IContractRepository>();
        var loggerMock = new Mock<ILogger<ContractService>>();
        _contractService = new ContractService(_mockRepo.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetContractByIdAsync_ShouldReturnContract_WhenContractExists()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var expectedContract = _fixture.CreateContract(
            id: contractId,
            description: "Special Project",
            state: ContractState.Accepted);

        _mockRepo.Setup(repo => repo.GetByIdAsync(contractId))
                 .ReturnsAsync(expectedContract);

        // Act
        var result = await _contractService.GetContractByIdAsync(contractId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedContract);
        result.ProjectDescription.Should().Be("Special Project");
        result.Status.Should().Be(ContractState.Accepted);
        _mockRepo.Verify(repo => repo.GetByIdAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task GetContractByIdAsync_ShouldReturnNull_WhenContractDoesNotExist()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.GetByIdAsync(contractId))
                 .ReturnsAsync((Contract?)null);

        // Act
        var result = await _contractService.GetContractByIdAsync(contractId);

        // Assert
        result.Should().BeNull();
        _mockRepo.Verify(repo => repo.GetByIdAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task GetAllByArtistIdAsync_ShouldReturnContracts()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        var contracts = _fixture.CreateContracts(3, artistId: artistId);

        _mockRepo.Setup(repo => repo.GetAllByArtistIdAsync(artistId))
                 .ReturnsAsync(contracts);

        // Act
        var result = await _contractService.GetAllByArtistIdAsync(artistId);

        // Assert
        var enumerable = result as Contract[] ?? result.ToArray();
        enumerable.Should().HaveCount(3);
        enumerable.All(c => c.ArtistId == artistId).Should().BeTrue();
        _mockRepo.Verify(repo => repo.GetAllByArtistIdAsync(artistId), Times.Once);
    }

    [Fact]
    public async Task GetAllByArtistIdAsync_ShouldReturnEmpty_WhenNoContracts()
    {
        // Arrange
        var artistId = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.GetAllByArtistIdAsync(artistId))
                 .ReturnsAsync(new List<Contract>());

        // Act
        var result = await _contractService.GetAllByArtistIdAsync(artistId);

        // Assert
        result.Should().BeEmpty();
        _mockRepo.Verify(repo => repo.GetAllByArtistIdAsync(artistId), Times.Once);
    }

    [Fact]
    public async Task GetAllByArtistIdAsync_WithDifferentStates_ShouldReturnFilteredByArtist()
    {
        // Arrange
        var (artistId, _, _, _) = _fixture.CreateMixedContracts(2, 2);
        
        var mixedStateContracts = new List<Contract>
        {
            _fixture.CreateContract(artistId: artistId, state: ContractState.Draft),
            _fixture.CreateContract(artistId: artistId, state: ContractState.Send),
            _fixture.CreateContract(artistId: artistId, state: ContractState.Accepted)
        };

        _mockRepo.Setup(repo => repo.GetAllByArtistIdAsync(artistId))
                 .ReturnsAsync(mixedStateContracts);

        // Act
        var result = await _contractService.GetAllByArtistIdAsync(artistId);

        // Assert
        var enumerable = result as Contract[] ?? result.ToArray();
        enumerable.Should().HaveCount(3);
        enumerable.Select(c => c.Status).Should().Contain([ContractState.Draft, ContractState.Send, ContractState.Accepted]);
        _mockRepo.Verify(repo => repo.GetAllByArtistIdAsync(artistId), Times.Once);
    }

    [Fact]
    public async Task GetAllByEmployerIdAsync_ShouldReturnContracts()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var contracts = _fixture.CreateContracts(2, employerId: employerId, state: ContractState.Rejected);

        _mockRepo.Setup(repo => repo.GetAllByEmployerIdAsync(employerId))
                 .ReturnsAsync(contracts);

        // Act
        var result = await _contractService.GetAllByEmployerIdAsync(employerId);

        // Assert
        var enumerable = result as Contract[] ?? result.ToArray();
        enumerable.Should().HaveCount(2);
        enumerable.All(c => c.EmployerId == employerId).Should().BeTrue();
        enumerable.All(c => c.Status == ContractState.Rejected).Should().BeTrue();
        _mockRepo.Verify(repo => repo.GetAllByEmployerIdAsync(employerId), Times.Once);
    }

    [Fact]
    public async Task GetAllByEmployerIdAsync_ShouldReturnEmpty_WhenNoContracts()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.GetAllByEmployerIdAsync(employerId))
                 .ReturnsAsync(new List<Contract>());

        // Act
        var result = await _contractService.GetAllByEmployerIdAsync(employerId);

        // Assert
        result.Should().BeEmpty();
        _mockRepo.Verify(repo => repo.GetAllByEmployerIdAsync(employerId), Times.Once);
    }

    [Fact]
    public async Task AddContractAsync_ShouldCallRepository()
    {
        // Arrange
        var contract = _fixture.CreateContract(
            description: "New Project",
            state: ContractState.Draft,
            startDate: DateTime.Today,
            endDate: DateTime.Today.AddDays(14));

        _mockRepo.Setup(r => r.AddAsync(
            contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            contract.Status, contract.StartDate, contract.EndDate))
                 .ReturnsAsync(contract.Id);

        // Act
        await _contractService.AddContractAsync(
            contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            contract.Status, contract.StartDate, contract.EndDate);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(
            contract.ArtistId, contract.EmployerId, "New Project", 
            ContractState.Draft, DateTime.Today, DateTime.Today.AddDays(14)), Times.Once);
    }

    [Fact]
    public async Task AddContractAsync_WithDifferentStates_ShouldCallRepository()
    {
        // Arrange
        var contract = _fixture.CreateContract(state: ContractState.Send);

        _mockRepo.Setup(r => r.AddAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), 
            It.IsAny<ContractState>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                 .ReturnsAsync(contract.Id);

        // Act
        await _contractService.AddContractAsync(
            contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            contract.Status, contract.StartDate, contract.EndDate);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(
            contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            ContractState.Send, contract.StartDate, contract.EndDate), Times.Once);
    }

    [Fact]
    public async Task UpdateContractAsync_ShouldCallRepository()
    {
        // Arrange
        var contract = _fixture.CreateContract(
            description: "Updated Project",
            state: ContractState.Accepted,
            startDate: DateTime.Today.AddDays(1),
            endDate: DateTime.Today.AddDays(15));

        _mockRepo.Setup(r => r.UpdateAsync(
            contract.Id, contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            contract.Status, contract.StartDate, contract.EndDate))
                 .Returns(Task.CompletedTask);

        // Act
        await _contractService.UpdateContractAsync(
            contract.Id, contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            contract.Status, contract.StartDate, contract.EndDate);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(
            contract.Id, contract.ArtistId, contract.EmployerId, "Updated Project", 
            ContractState.Accepted, DateTime.Today.AddDays(1), DateTime.Today.AddDays(15)), Times.Once);
    }

    [Fact]
    public async Task UpdateContractAsync_WithRejectedState_ShouldCallRepository()
    {
        // Arrange
        var contract = _fixture.CreateContract(state: ContractState.Rejected);

        _mockRepo.Setup(r => r.UpdateAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), 
            It.IsAny<ContractState>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                 .Returns(Task.CompletedTask);

        // Act
        await _contractService.UpdateContractAsync(
            contract.Id, contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            contract.Status, contract.StartDate, contract.EndDate);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(
            contract.Id, contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            ContractState.Rejected, contract.StartDate, contract.EndDate), Times.Once);
    }

    [Fact]
    public async Task DeleteContractAsync_ShouldCallRepository()
    {
        // Arrange
        var contract = _fixture.CreateContract();
        _mockRepo.Setup(r => r.DeleteAsync(contract.Id)).Returns(Task.CompletedTask);

        // Act
        await _contractService.DeleteContractAsync(contract.Id);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(contract.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteContractAsync_WithSpecificContract_ShouldCallRepository()
    {
        // Arrange
        var contract = _fixture.CreateContract(
            description: "Contract to delete",
            state: ContractState.Draft);

        _mockRepo.Setup(r => r.DeleteAsync(contract.Id)).Returns(Task.CompletedTask);

        // Act
        await _contractService.DeleteContractAsync(contract.Id);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(contract.Id), Times.Once);
    }

    [Fact]
    public async Task GetContractByIdAsync_ShouldThrow_WhenRepositoryThrows()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var exception = new Exception("DB error");

        _mockRepo.Setup(r => r.GetByIdAsync(contractId)).ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _contractService.GetContractByIdAsync(contractId));
        _mockRepo.Verify(r => r.GetByIdAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task AddContractAsync_ShouldThrow_WhenRepositoryThrows()
    {
        // Arrange
        var contract = _fixture.CreateContract();
        var exception = new Exception("Add failed");

        _mockRepo.Setup(r => r.AddAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), 
            It.IsAny<ContractState>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                 .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _contractService.AddContractAsync(
            contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            contract.Status, contract.StartDate, contract.EndDate));
    }

    [Fact]
    public async Task UpdateContractAsync_ShouldThrow_WhenRepositoryThrows()
    {
        // Arrange
        var contract = _fixture.CreateContract();
        var exception = new Exception("Update failed");

        _mockRepo.Setup(r => r.UpdateAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), 
            It.IsAny<ContractState>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                 .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _contractService.UpdateContractAsync(
            contract.Id, contract.ArtistId, contract.EmployerId, contract.ProjectDescription, 
            contract.Status, contract.StartDate, contract.EndDate));
    }

    [Fact]
    public async Task DeleteContractAsync_ShouldThrow_WhenRepositoryThrows()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var exception = new Exception("Delete failed");

        _mockRepo.Setup(r => r.DeleteAsync(contractId)).ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _contractService.DeleteContractAsync(contractId));
    }
}