using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Tests.Common;
using ArtLink.Tests.Common.Database;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.IntegrationTests.Repositories;

[Collection("Database collection")]
[Trait("Category", TestCategories.Integration)]
public class EmployerRepositoryIt(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private EmployerRepository CreateRepository()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseNpgsql(db.ConnectionString)
            .Options;

        var context = new ArtLinkDbContext(options);
        return new EmployerRepository(context, new NullLogger<EmployerRepository>());
    }

    [Fact]
    public async Task Employer_FullCycle_WithSearchAndLogin()
    {
        // Arrange
        var repo = CreateRepository();

        // Act 1 — добавить работодателя
        var newEmployerId = await repo.AddAsync(
            "Test Company",
            "test.company@example.com",
            "hashed_password_123",
            "John",
            "Doe"
        );

        // Assert 1 — проверить что добавлен
        var added = await repo.GetByIdAsync(newEmployerId);
        added.Should().NotBeNull();
        added.CompanyName.Should().Be("Test Company");
        added.Email.Should().Be("test.company@example.com");
        added.CpFirstName.Should().Be("John");
        added.CpLastName.Should().Be("Doe");

        // Act 2 — обновить работодателя
        await repo.UpdateAsync(
            newEmployerId,
            "Updated Company",
            "updated.company@example.com",
            "Jane",
            "Smith"
        );

        // Assert 2 — проверить обновление
        var updated = await repo.GetByIdAsync(newEmployerId);
        updated.Should().NotBeNull();
        updated.CompanyName.Should().Be("Updated Company");
        updated.Email.Should().Be("updated.company@example.com");
        updated.CpFirstName.Should().Be("Jane");
        updated.CpLastName.Should().Be("Smith");

        // Act 3 — поиск по компании
        var searchByCompany = await repo.SearchByPromptAsync("Updated");
        searchByCompany.Should().Contain(e => e.Id == newEmployerId);

        // Act 4 — поиск по имени контактного лица
        var searchByName = await repo.SearchByPromptAsync("Jane");
        searchByName.Should().Contain(e => e.Id == newEmployerId);

        // Act 5 — получить всех работодателей
        var allEmployers = await repo.GetAllEmployersAsync();
        allEmployers.Should().Contain(e => e.Id == newEmployerId);

        // Act 6 — попытка логина с правильными данными
        var loginSuccess = await repo.LoginAsync("updated.company@example.com", "hashed_password_123");
        loginSuccess.Should().NotBeNull();
        loginSuccess.Id.Should().Be(newEmployerId);

        // Act 7 — попытка логина с неправильным паролем
        var loginFail = await repo.LoginAsync("updated.company@example.com", "wrong_password");
        loginFail.Should().BeNull();

        // Act 8 — удалить работодателя
        await repo.DeleteAsync(newEmployerId);

        // Assert 8 — проверить удаление
        var deleted = await repo.GetByIdAsync(newEmployerId);
        deleted.Should().BeNull();
    }
}