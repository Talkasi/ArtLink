using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtLink.Tests.Repositories;

public class EmployerRepositoryTests
{
    private ArtLinkDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ArtLinkDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEmployer()
    {
        using var context = CreateContext();
        var repo = new EmployerRepository(context);

        await repo.AddAsync("TechCorp", "hr@techcorp.com", "Alice", "Johnson", "hash123");

        var employer = await context.Employers.FirstOrDefaultAsync();
        Assert.NotNull(employer);
        Assert.Equal("TechCorp", employer.CompanyName);
        Assert.Equal("Alice", employer.CpFirstName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEmployer()
    {
        using var context = CreateContext();
        var repo = new EmployerRepository(context);

        var id = Guid.NewGuid();
        var employer = new EmployerDb(id, "Media Inc", "media@corp.com", "passhash", "Bob", "Smith");
        context.Employers.Add(employer);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Media Inc", result!.CompanyName);
    }

    [Fact]
    public async Task GetAllEmployersAsync_ShouldReturnAllEmployers()
    {
        using var context = CreateContext();
        var repo = new EmployerRepository(context);

        context.Employers.AddRange(
            new EmployerDb(Guid.NewGuid(), "A", "a@a.com", "hash1", "John", "Doe"),
            new EmployerDb(Guid.NewGuid(), "B", "b@b.com", "hash2", "Jane", "Smith")
        );
        await context.SaveChangesAsync();

        var result = (await repo.GetAllEmployersAsync()).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyEmployer()
    {
        using var context = CreateContext();
        var repo = new EmployerRepository(context);

        var id = Guid.NewGuid();
        var employer = new EmployerDb(id, "OldName", "old@mail.com", "hash", "OldFirst", "OldLast");
        context.Employers.Add(employer);
        await context.SaveChangesAsync();

        await repo.UpdateAsync(id, "NewName", "new@mail.com", "NewFirst", "NewLast");

        var updated = await context.Employers.FindAsync(id);
        Assert.NotNull(updated);
        Assert.Equal("NewName", updated!.CompanyName);
        Assert.Equal("NewFirst", updated.CpFirstName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEmployer()
    {
        using var context = CreateContext();
        var repo = new EmployerRepository(context);

        var id = Guid.NewGuid();
        context.Employers.Add(new EmployerDb(id, "ToDelete", "del@mail.com", "hash", "Del", "User"));
        await context.SaveChangesAsync();

        await repo.DeleteAsync(id);

        var deleted = await context.Employers.FindAsync(id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task SearchByPromptAsync_ShouldReturnMatches()
    {
        using var context = CreateContext();
        var repo = new EmployerRepository(context);

        context.Employers.AddRange(
            new EmployerDb(Guid.NewGuid(), "AlphaTech", "a@a.com", "hash", "Lena", "Karlson"),
            new EmployerDb(Guid.NewGuid(), "BetaSoft", "b@b.com", "hash", "Tom", "Hansen"),
            new EmployerDb(Guid.NewGuid(), "ZetaCorp", "z@z.com", "hash", "Hank", "Beta")
        );
        await context.SaveChangesAsync();

        var results = (await repo.SearchByPromptAsync("Beta")).ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, e => e.CompanyName == "BetaSoft");
        Assert.Contains(results, e => e.CpLastName == "Beta");
    }
}

