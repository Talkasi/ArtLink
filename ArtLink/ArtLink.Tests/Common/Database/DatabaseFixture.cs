using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ArtLink.Tests.Common.Database;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly string _baseConnectionString;
    private readonly string _databaseName;
    private readonly string _scriptsPath;

    public string ConnectionString { get; private set; }

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

            _baseConnectionString = config.GetConnectionString("TestDatabase")
                                    ?? throw new InvalidOperationException("Connection string 'TestDatabase' not found in appsettings.Test.json");
        }
        else
        {
            _baseConnectionString = tmpConnectionString;
        }
        
        _databaseName = $"test_db_{Guid.NewGuid():N}";
        ConnectionString = $"{_baseConnectionString};Database={_databaseName}";
        
        _scriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "Common", "Database", "schemas");
    }

    public async Task InitializeAsync()
    {
        await CreateDatabaseAsync();
        await ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await DropDatabaseAsync();
    }

    private async Task CreateDatabaseAsync()
    {
        var masterConnString = new NpgsqlConnectionStringBuilder(_baseConnectionString)
        {
            Database = "postgres"
        }.ToString();

        await using var conn = new NpgsqlConnection(masterConnString);
        await conn.OpenAsync();
        
        var createDbSql = $"CREATE DATABASE {_databaseName}";
        await using var cmd = new NpgsqlCommand(createDbSql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task DropDatabaseAsync()
    {
        var masterConnString = new NpgsqlConnectionStringBuilder(_baseConnectionString)
        {
            Database = "postgres"
        }.ToString();

        await using var conn = new NpgsqlConnection(masterConnString);
        await conn.OpenAsync();
        
        var terminateSql = $@"
            SELECT pg_terminate_backend(pid) 
            FROM pg_stat_activity 
            WHERE datname = '{_databaseName}' AND pid <> pg_backend_pid()";
        
        await using var terminateCmd = new NpgsqlCommand(terminateSql, conn);
        await terminateCmd.ExecuteNonQueryAsync();
        
        var dropDbSql = $"DROP DATABASE IF EXISTS {_databaseName}";
        await using var dropCmd = new NpgsqlCommand(dropDbSql, conn);
        await dropCmd.ExecuteNonQueryAsync();
    }

    private async Task ApplyScriptAsync(string scriptFile)
    {
        var fullPath = Path.Combine(_scriptsPath, scriptFile);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Script not found: {fullPath}");

        var sql = await File.ReadAllTextAsync(fullPath);

        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await ApplyScriptAsync("reset.sql");
        await ApplyScriptAsync("create.sql");
        await ApplyScriptAsync("init_data.sql");
    }
}