using System.Data.Common;
using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;

namespace ArtLink.Tests.Common.Database;

public abstract class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    public DbConnection Connection { get; private set; } = null!;

    public DatabaseFixture()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("artlink_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        Connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
        await Connection.OpenAsync();

        var schemaSql = await File.ReadAllTextAsync("schemas/create.sql");
        await Connection.ExecuteAsync(schemaSql);

        if (File.Exists("schemas/testdata.sql"))
        {
            var testDataSql = await File.ReadAllTextAsync("schemas/testdata.sql");
            await Connection.ExecuteAsync(testDataSql);
        }
    }

    public async Task ResetDatabaseAsync()
    {
        var schemaSql = await File.ReadAllTextAsync("schemas/create.sql");
        await Connection.ExecuteAsync("DROP SCHEMA public CASCADE; CREATE SCHEMA public;");
        await Connection.ExecuteAsync(schemaSql);

        if (File.Exists("schemas/testdata.sql"))
        {
            var testDataSql = await File.ReadAllTextAsync("schemas/testdata.sql");
            await Connection.ExecuteAsync(testDataSql);
        }
    }

    public async Task DisposeAsync()
    {
        await Connection.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
