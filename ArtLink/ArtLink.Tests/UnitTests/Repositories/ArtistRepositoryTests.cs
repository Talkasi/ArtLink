using Allure.Xunit.Attributes;
using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using ArtLink.Tests.Common.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.UnitTests.Repositories;

[AllureSuite("Artist Repository Tests")]
public class ArtistRepositoryUnitTests(ArtistFixture fixture) : IClassFixture<ArtistFixture>
{
    private static ArtistRepository GetInMemoryRepository()
    {
        var options = new DbContextOptionsBuilder<ArtLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ArtLinkDbContext(options);
        var logger = NullLogger<ArtistRepository>.Instance;
        return new ArtistRepository(context, logger);
    }

    [Fact]
    [AllureFeature("AddAsync + GetByIdAsync")]
    [AllureStory("Positive case - artist added and retrieved")]
    public async Task AddAndGetById_ShouldReturnCorrectArtist()
    {
        // Arrange
        var repository = GetInMemoryRepository();
        var artist = fixture.CreateArtist();

        var id = await repository.AddAsync(
            artist.FirstName, 
            artist.LastName, 
            artist.Email, 
            artist.PasswordHash!, 
            artist.Bio, 
            artist.ProfilePicturePath, 
            artist.Experience);

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(artist.FirstName);
        result.LastName.Should().Be(artist.LastName);
        result.Email.Should().Be(artist.Email);
    }

    [Fact]
    [AllureFeature("GetAllAsync")]
    [AllureStory("Positive case - multiple artists")]
    public async Task GetAll_ShouldReturnAllArtists()
    {
        var repository = GetInMemoryRepository();

        for (int i = 0; i < 3; i++)
        {
            var artist = fixture.CreateArtist(firstName: $"Artist{i}");
            await repository.AddAsync(
                artist.FirstName, 
                artist.LastName, 
                artist.Email, 
                artist.PasswordHash!, 
                artist.Bio, 
                artist.ProfilePicturePath, 
                artist.Experience);
        }

        var all = await repository.GetAllAsync();
        all.Should().HaveCount(3);
    }

    [Fact]
    [AllureFeature("UpdateAsync")]
    [AllureStory("Positive case - update artist")]
    public async Task Update_ShouldChangeArtistFields()
    {
        var repository = GetInMemoryRepository();
        var artist = fixture.CreateArtist();
        var id = await repository.AddAsync(
            artist.FirstName, 
            artist.LastName, 
            artist.Email, 
            artist.PasswordHash!, 
            artist.Bio, 
            artist.ProfilePicturePath, 
            artist.Experience);

        // Act
        await repository.UpdateAsync(id, "UpdatedFirst", "UpdatedLast", "updated@example.com", "Updated bio", "/new.jpg", 10);

        var updated = await repository.GetByIdAsync(id);

        // Assert
        updated!.FirstName.Should().Be("UpdatedFirst");
        updated.LastName.Should().Be("UpdatedLast");
        updated.Email.Should().Be("updated@example.com");
        updated.Bio.Should().Be("Updated bio");
        updated.ProfilePicturePath.Should().Be("/new.jpg");
        updated.Experience.Should().Be(10);
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Positive case - delete artist")]
    public async Task Delete_ShouldRemoveArtist()
    {
        var repository = GetInMemoryRepository();
        var artist = fixture.CreateArtist();
        var id = await repository.AddAsync(
            artist.FirstName, 
            artist.LastName, 
            artist.Email, 
            artist.PasswordHash!, 
            artist.Bio, 
            artist.ProfilePicturePath, 
            artist.Experience);

        await repository.DeleteAsync(id);
        var deleted = await repository.GetByIdAsync(id);

        deleted.Should().BeNull();
    }

    [Fact]
    [AllureFeature("SearchByPromptAsync")]
    [AllureStory("Positive case - search artists")]
    public async Task SearchByPrompt_ShouldReturnMatchingArtists()
    {
        var repository = GetInMemoryRepository();
        
        var artist1 = fixture.CreateArtist(firstName: "Anna", lastName: "Taylor", email: "anna@example.com");
        var artist2 = fixture.CreateArtist(firstName: "Bob", lastName: "Builder", email: "bob@example.com");
        
        await repository.AddAsync(artist1.FirstName, artist1.LastName, artist1.Email, "pass", null, null, null);
        await repository.AddAsync(artist2.FirstName, artist2.LastName, artist2.Email, "pass", null, null, null);

        var results = (await repository.SearchByPromptAsync("Ann")).ToList();

        results.Should().HaveCount(1);
        results[0].FirstName.Should().Be("Anna");
    }

    [Fact]
    [AllureFeature("SearchByPromptAsync")]
    [AllureStory("Negative case - no matches")]
    public async Task SearchByPrompt_NoMatch_ShouldReturnEmpty()
    {
        var repository = GetInMemoryRepository();
        var artist = fixture.CreateArtist(firstName: "Anna", lastName: "Taylor", email: "anna@example.com");
        await repository.AddAsync(artist.FirstName, artist.LastName, artist.Email, "pass", null, null, null);

        var results = await repository.SearchByPromptAsync("ZZZ");

        results.Should().BeEmpty();
    }

    [Fact]
    [AllureFeature("LoginAsync")]
    [AllureStory("Positive case - login success")]
    public async Task Login_ShouldReturnArtist_WhenCredentialsMatch()
    {
        var repository = GetInMemoryRepository();
        var artist = fixture.CreateArtist(email: "login@test.com");
        await repository.AddAsync(
            artist.FirstName, 
            artist.LastName, 
            artist.Email, 
            artist.PasswordHash!, 
            artist.Bio, 
            artist.ProfilePicturePath, 
            artist.Experience);

        var result = await repository.LoginAsync(artist.Email, artist.PasswordHash!);

        result.Should().NotBeNull();
        result.Email.Should().Be("login@test.com");
    }

    [Fact]
    [AllureFeature("LoginAsync")]
    [AllureStory("Negative case - login fail")]
    public async Task Login_ShouldReturnNull_WhenCredentialsIncorrect()
    {
        var repository = GetInMemoryRepository();
        var artist = fixture.CreateArtist(email: "login@test.com");
        await repository.AddAsync(
            artist.FirstName, 
            artist.LastName, 
            artist.Email, 
            artist.PasswordHash!, 
            artist.Bio, 
            artist.ProfilePicturePath, 
            artist.Experience);

        var result = await repository.LoginAsync(artist.Email, "wrongpass");

        result.Should().BeNull();
    }

    [Fact]
    [AllureFeature("GetByIdAsync")]
    [AllureStory("Exception handling - non-existent id update")]
    public async Task Update_NonExistentId_ShouldNotThrow()
    {
        var repository = GetInMemoryRepository();
        var nonExistentId = Guid.NewGuid();

        Func<Task> act = async () => await repository.UpdateAsync(nonExistentId, "x", "y", "z@example.com", null, null, null);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    [AllureFeature("DeleteAsync")]
    [AllureStory("Exception handling - non-existent id delete")]
    public async Task Delete_NonExistentId_ShouldNotThrow()
    {
        var repository = GetInMemoryRepository();
        var nonExistentId = Guid.NewGuid();

        Func<Task> act = async () => await repository.DeleteAsync(nonExistentId);

        await act.Should().NotThrowAsync();
    }
}