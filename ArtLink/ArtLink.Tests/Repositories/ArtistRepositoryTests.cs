using Microsoft.EntityFrameworkCore;
using ArtLink.DataAccess.Context;
using ArtLink.DataAccess.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArtLink.Tests.Repositories
{
    public class ArtistRepositoryTests
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
        public async Task AddAndGetByIdAsync_ShouldReturnCorrectArtist()
        {
            // Arrange
            var repository = GetInMemoryRepository();
            const string firstName = "John";
            const string lastName = "Doe";
            const string email = "john.doe@example.com";
            const string passwordHash = "hashed_pass";
            const string bio = "I am an artist";
            const string profilePicturePath = "profile.jpg";
            const int experience = 5;

            await repository.AddAsync(firstName, lastName, email, passwordHash, bio, profilePicturePath, experience);

            var all = await repository.GetAllAsync();
            var added = all.FirstOrDefault();

            // Act
            var result = await repository.GetByIdAsync(added!.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateArtist()
        {
            var repository = GetInMemoryRepository();

            await repository.AddAsync("Alice", "Smith", "alice@example.com", "pass", "bio", "path.jpg", 2);
            var artist = (await repository.GetAllAsync()).First();

            await repository.UpdateAsync(artist.Id, "AliceUpdated", "SmithUpdated", "alice@new.com", "new bio", "newpath.jpg", 10);

            var updated = await repository.GetByIdAsync(artist.Id);

            Assert.NotNull(updated);
            Assert.Equal("AliceUpdated", updated.FirstName);
            Assert.Equal("SmithUpdated", updated.LastName);
            Assert.Equal("alice@new.com", updated.Email);
            Assert.Equal("new bio", updated.Bio);
            Assert.Equal("newpath.jpg", updated.ProfilePicturePath);
            Assert.Equal(10, updated.Experience);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveArtist()
        {
            var repository = GetInMemoryRepository();

            await repository.AddAsync("Delete", "Me", "deleteme@example.com", "pass", null, null, null);
            var artist = (await repository.GetAllAsync()).First();

            await repository.DeleteAsync(artist.Id);
            var deleted = await repository.GetByIdAsync(artist.Id);

            Assert.Null(deleted);
        }

        [Fact]
        public async Task SearchByPromptAsync_ShouldReturnMatchingArtists()
        {
            var repository = GetInMemoryRepository();

            await repository.AddAsync("Anna", "Taylor", "anna@example.com", "pass", null, null, null);
            await repository.AddAsync("Bob", "Builder", "bob@example.com", "pass", null, null, null);

            var results = (await repository.SearchByPromptAsync("Ann")).ToList();

            Assert.Single(results);
            Assert.Equal("Anna", results[0].FirstName);
        }
    }
}
