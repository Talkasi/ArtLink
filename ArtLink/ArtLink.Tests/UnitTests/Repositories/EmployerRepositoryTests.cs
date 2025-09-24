using Allure.Xunit.Attributes;
using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Models;
using ArtLink.DataAccess.Repositories;
using ArtLink.Domain.Models;
using ArtLink.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.UnitTests.Repositories;

[AllureSuite("Employer Repository Tests")]
public class EmployerRepositoryUnitTests(EmployerFixture fixture) : IClassFixture<EmployerFixture>
{
    private static ArtLinkDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ArtLinkDbContext(options);
    }

    private static EmployerRepository CreateRepository(ArtLinkDbContext context)
    {
        return new EmployerRepository(context, NullLogger<EmployerRepository>.Instance);
    }

    [Fact]
    [AllureFeature("AddAsync")]
    [AllureStory("Positive case - add employer")]
    public async Task AddAsync_ShouldAddEmployer()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var employer = fixture.CreateEmployer(
            companyName: "TechCorp", 
            email: "hr@techcorp.com",
            cpFirstName: "Alice",
            cpLastName: "Johnson");

        var id = await repo.AddAsync(
            employer.CompanyName, 
            employer.Email, 
            employer.PasswordHash, 
            employer.CpFirstName, 
            employer.CpLastName);

        var added = await context.Employers.FindAsync(id);
        added.Should().NotBeNull();
        added.CompanyName.Should().Be("TechCorp");
        added.CpFirstName.Should().Be("Alice");
    }

    [Fact]
    [AllureFeature("GetByIdAsync")]
    [AllureStory("Positive case - get employer by id")]
    public async Task GetByIdAsync_ShouldReturnEmployer()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var employer = fixture.CreateEmployer(companyName: "Media Inc");
        await context.Employers.AddAsync(new EmployerDb(
            employer.Id, 
            employer.CompanyName, 
            employer.Email, 
            employer.PasswordHash, 
            employer.CpFirstName, 
            employer.CpLastName));
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(employer.Id);

        result.Should().NotBeNull();
        result.CompanyName.Should().Be("Media Inc");
    }

    [Fact]
    [AllureFeature("GetAllEmployersAsync")]
    [AllureStory("Positive case - get all employers")]
    public async Task GetAllEmployersAsync_ShouldReturnAll()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var employers = fixture.CreateEmployers(2);
        employers[0] = fixture.CreateEmployer(companyName: "A", email: "a@a.com", cpFirstName: "John", cpLastName: "Doe");
        employers[1] = fixture.CreateEmployer(companyName: "B", email: "b@b.com", cpFirstName: "Jane", cpLastName: "Smith");

        await context.Employers.AddRangeAsync(employers.Select(e => 
            new EmployerDb(e.Id, e.CompanyName, e.Email, e.PasswordHash, e.CpFirstName, e.CpLastName)));
        await context.SaveChangesAsync();

        var result = (await repo.GetAllEmployersAsync()).ToList();

        result.Should().HaveCount(2);
        result.Should().Contain(e => e.CompanyName == "A");
        result.Should().Contain(e => e.CompanyName == "B");
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Positive case - update employer")]
    public async Task UpdateAsync_ShouldModifyEmployer()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var employer = fixture.CreateEmployer(
            companyName: "OldName", 
            email: "old@mail.com",
            cpFirstName: "OldFirst",
            cpLastName: "OldLast");
            
        await context.Employers.AddAsync(new EmployerDb(
            employer.Id, 
            employer.CompanyName, 
            employer.Email, 
            employer.PasswordHash, 
            employer.CpFirstName, 
            employer.CpLastName));
        await context.SaveChangesAsync();

        await repo.UpdateAsync(employer.Id, "NewName", "new@mail.com", "NewFirst", "NewLast");

        var updated = await context.Employers.FindAsync(employer.Id);
        updated.Should().NotBeNull();
        updated.CompanyName.Should().Be("NewName");
        updated.CpFirstName.Should().Be("NewFirst");
        updated.CpLastName.Should().Be("NewLast");
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Positive case - delete employer")]
    public async Task DeleteAsync_ShouldRemoveEmployer()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var employer = fixture.CreateEmployer(companyName: "ToDelete");
        await context.Employers.AddAsync(new EmployerDb(
            employer.Id, 
            employer.CompanyName, 
            employer.Email, 
            employer.PasswordHash, 
            employer.CpFirstName, 
            employer.CpLastName));
        await context.SaveChangesAsync();

        await repo.DeleteAsync(employer.Id);

        var deleted = await context.Employers.FindAsync(employer.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    [AllureFeature("SearchByPromptAsync")]
    [AllureStory("Positive case - search employers by prompt")]
    public async Task SearchByPromptAsync_ShouldReturnMatchingEmployers()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var employers = new List<Employer>
        {
            fixture.CreateEmployer(companyName: "AlphaTech", cpFirstName: "Lena", cpLastName: "Karlson"),
            fixture.CreateEmployer(companyName: "BetaSoft", cpFirstName: "Tom", cpLastName: "Hansen"),
            fixture.CreateEmployer(companyName: "ZetaCorp", cpFirstName: "Hank", cpLastName: "Beta")
        };

        await context.Employers.AddRangeAsync(employers.Select(e => 
            new EmployerDb(e.Id, e.CompanyName, e.Email, e.PasswordHash, e.CpFirstName, e.CpLastName)));
        await context.SaveChangesAsync();

        var results = (await repo.SearchByPromptAsync("Beta")).ToList();

        results.Should().HaveCount(2);
        results.Should().Contain(e => e.CompanyName == "BetaSoft");
        results.Should().Contain(e => e.CpLastName == "Beta");
    }

    [Fact]
    [AllureFeature("LoginAsync")]
    [AllureStory("Positive case - login success")]
    public async Task LoginAsync_ShouldReturnEmployer_WhenCredentialsCorrect()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var employer = fixture.CreateEmployer(
            companyName: "LoginCorp", 
            email: "login@mail.com",
            cpFirstName: "Sam",
            cpLastName: "Wilson");

        await context.Employers.AddAsync(new EmployerDb(
            employer.Id, 
            employer.CompanyName, 
            employer.Email, 
            employer.PasswordHash, 
            employer.CpFirstName, 
            employer.CpLastName));
        await context.SaveChangesAsync();

        var result = await repo.LoginAsync("login@mail.com", employer.PasswordHash);

        result.Should().NotBeNull();
        result.CompanyName.Should().Be("LoginCorp");
    }

    [Fact]
    [AllureFeature("LoginAsync")]
    [AllureStory("Negative case - login failure")]
    public async Task LoginAsync_ShouldReturnNull_WhenCredentialsIncorrect()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.LoginAsync("wrong@mail.com", "wronghash");

        result.Should().BeNull();
    }

    [Fact]
    [AllureFeature("GetByIdAsync")]
    [AllureStory("Negative case - non-existent employer")]
    public async Task GetByIdAsync_NonExistent_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Exception handling - update non-existent employer")]
    public async Task UpdateAsync_NonExistent_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        Func<Task> act = async () => await repo.UpdateAsync(
            Guid.NewGuid(), "NewName", "new@mail.com", "NewFirst", "NewLast");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Exception handling - delete non-existent employer")]
    public async Task DeleteAsync_NonExistent_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        Func<Task> act = async () => await repo.DeleteAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("SearchByPromptAsync")]
    [AllureStory("Negative case - no matching employers")]
    public async Task SearchByPromptAsync_NoMatches_ShouldReturnEmpty()
    {
        await using var context = CreateContext();
        var repo = CreateRepository(context);

        var employer = fixture.CreateEmployer(companyName: "TestCompany");
        await context.Employers.AddAsync(new EmployerDb(
            employer.Id, 
            employer.CompanyName, 
            employer.Email, 
            employer.PasswordHash, 
            employer.CpFirstName, 
            employer.CpLastName));
        await context.SaveChangesAsync();

        var results = await repo.SearchByPromptAsync("NonExistent");

        results.Should().BeEmpty();
    }
}