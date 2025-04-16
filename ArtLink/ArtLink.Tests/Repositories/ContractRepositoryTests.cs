using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models.Enums;
using ArtLink.DataAccess.Repositories;
using ArtLink.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.Repositories;

public class ContractRepositoryTests
{
    private static ArtLinkDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ArtLinkDbContext(options);
    }

    private static ContractRepository CreateRepository(ArtLinkDbContext context)
    {
        return new ContractRepository(context, NullLogger<ContractRepository>.Instance);
    }

    [Fact]
    public async Task AddAsync_ShouldAddContract()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var artistId = Guid.NewGuid();
        var employerId = Guid.NewGuid();

        await repo.AddAsync(artistId, employerId, "Project X", status: ContractState.Accepted, startDate: DateTime.Today, endDate: DateTime.Today.AddDays(30));

        var contract = await context.Contracts.FirstOrDefaultAsync();
        Assert.NotNull(contract);
        Assert.Equal(artistId, contract.ArtistId);
        Assert.Equal("Project X", contract.ProjectDescription);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectContract()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var id = Guid.NewGuid();

        context.Contracts.Add(new DataAccess.Models.ContractDb(
            id, Guid.NewGuid(), Guid.NewGuid(), "Project A", DateTime.Today, DateTime.Today.AddDays(10),
            ContractStateDb.Accepted));
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(id);
        Assert.NotNull(result);
        Assert.Equal("Project A", result.ProjectDescription);
    }

    [Fact]
    public async Task GetAllByArtistIdAsync_ShouldReturnContracts()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var artistId = Guid.NewGuid();

        context.Contracts.AddRange(
            new DataAccess.Models.ContractDb(Guid.NewGuid(), artistId, Guid.NewGuid(), "A", DateTime.Now, DateTime.Now, ContractStateDb.Accepted),
            new DataAccess.Models.ContractDb(Guid.NewGuid(), artistId, Guid.NewGuid(), "B", DateTime.Now, DateTime.Now),
            new DataAccess.Models.ContractDb(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "C", DateTime.Now, DateTime.Now, ContractStateDb.Rejected)
        );
        await context.SaveChangesAsync();

        var contracts = (await repo.GetAllByArtistIdAsync(artistId)).ToList();
        Assert.Equal(2, contracts.Count);
    }

    [Fact]
    public async Task GetAllByEmployerIdAsync_ShouldReturnContracts()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var employerId = Guid.NewGuid();

        context.Contracts.AddRange(
            new DataAccess.Models.ContractDb(Guid.NewGuid(), Guid.NewGuid(), employerId, "A", DateTime.Now, DateTime.Now, ContractStateDb.Accepted),
            new DataAccess.Models.ContractDb(Guid.NewGuid(), Guid.NewGuid(), employerId, "B", DateTime.Now, DateTime.Now),
            new DataAccess.Models.ContractDb(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "C", DateTime.Now, DateTime.Now, ContractStateDb.Rejected)
        );
        await context.SaveChangesAsync();

        var contracts = (await repo.GetAllByEmployerIdAsync(employerId)).ToList();
        Assert.Equal(2, contracts.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateContract()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var contractId = Guid.NewGuid();
        context.Contracts.Add(new DataAccess.Models.ContractDb(contractId, Guid.NewGuid(), Guid.NewGuid(), "Old", DateTime.Today, DateTime.Today, ContractStateDb.Accepted));
        await context.SaveChangesAsync();

        var newArtistId = Guid.NewGuid();
        var newEmployerId = Guid.NewGuid();

        await repo.UpdateAsync(contractId, newArtistId, newEmployerId, "Updated Desc", status: ContractState.Accepted, startDate: DateTime.Today.AddDays(1), endDate: DateTime.Today.AddDays(5));

        var updated = await context.Contracts.FindAsync(contractId);
        Assert.NotNull(updated);
        Assert.Equal("Updated Desc", updated.ProjectDescription);
        Assert.Equal(ContractStateDb.Accepted, updated.Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveContract()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var contractId = Guid.NewGuid();

        context.Contracts.Add(new DataAccess.Models.ContractDb(contractId, Guid.NewGuid(), Guid.NewGuid(), "To Delete", DateTime.Now, DateTime.Now,
            ContractStateDb.Rejected));
        await context.SaveChangesAsync();

        await repo.DeleteAsync(contractId);

        var contract = await context.Contracts.FindAsync(contractId);
        Assert.Null(contract);
    }
}
