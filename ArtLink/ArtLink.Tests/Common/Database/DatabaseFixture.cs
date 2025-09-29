using Microsoft.Extensions.Configuration;

namespace ArtLink.Tests.Common.Database;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly string _connectionString;
    private readonly string _scriptsPath;

    public string ConnectionString => _connectionString;

    public DatabaseFixture()
    {
        var tmpConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        
        if (string.IsNullOrEmpty(tmpConnectionString))
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            _connectionString = config.GetConnectionString("TestDatabase")
                                ?? throw new InvalidOperationException("Connection string 'TestDatabase' not found in appsettings.Test.json");
        }
        else
        {
            _connectionString = tmpConnectionString;
        }
        
        _scriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "Common", "Database", "schemas");
    }

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    private async Task ApplyScriptAsync(string scriptFile)
    {
        var fullPath = Path.Combine(_scriptsPath, scriptFile);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Script not found: {fullPath}");

        var sql = await File.ReadAllTextAsync(fullPath);

        await using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new Npgsql.NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await ApplyScriptAsync("reset.sql");
        await ApplyScriptAsync("create.sql");
        await ApplyScriptAsync("init_data.sql");
    }
}
