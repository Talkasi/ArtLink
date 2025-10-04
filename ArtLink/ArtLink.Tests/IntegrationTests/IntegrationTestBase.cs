using ArtLink.Tests.Common.Database;

namespace ArtLink.Tests.IntegrationTests
{
    [Collection("Database collection")]
    public abstract class IntegrationTestBase(DatabaseFixture fixture) : IAsyncLifetime
    {
        protected readonly DatabaseFixture Fixture = fixture;

        public Task InitializeAsync()
        {
            return Fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
