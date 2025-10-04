using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Services.Employer;
using ArtLink.Tests.Common;
using ArtLink.Tests.Common.Database;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Services;

[Collection("Database collection")]
[Trait("Category", TestCategories.Integration)]
public class EmployerServiceIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private readonly EmployerFixture _fixture = new EmployerFixture();

    private EmployerService CreateService()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        var repository = new EmployerRepository(context, new NullLogger<EmployerRepository>());
        return new EmployerService(repository, new NullLogger<EmployerService>());
    }

    [Fact]
    public async Task Employer_FullCycle_UsingFixtureData()
    {
        var service = CreateService();

        var testEmployer = _fixture.CreateEmployer(); 

        var employerId = await service.AddEmployerAsync(
            testEmployer.CompanyName,
            testEmployer.Email,
            testEmployer.PasswordHash,
            testEmployer.CpFirstName,
            testEmployer.CpLastName
        );

        var added = await service.GetEmployerByIdAsync(employerId);
        added.Should().NotBeNull();
        added.CompanyName.Should().Be(testEmployer.CompanyName);

        await service.UpdateEmployerAsync(
            employerId,
            "Updated " + testEmployer.CompanyName,
            testEmployer.Email,
            testEmployer.CpFirstName,
            testEmployer.CpLastName
        );

        var updated = await service.GetEmployerByIdAsync(employerId);
        updated?.CompanyName.Should().Be("Updated " + testEmployer.CompanyName);

        var loginResult = await service.LoginEmployerAsync(testEmployer.Email, testEmployer.PasswordHash);
        loginResult.Should().NotBeNull();
        loginResult.Id.Should().Be(employerId);

        await service.DeleteEmployerAsync(employerId);

        var deleted = await service.GetEmployerByIdAsync(employerId);
        deleted.Should().BeNull();
    }
}
