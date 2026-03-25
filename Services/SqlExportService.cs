using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Parquet.Schema;

namespace DataView;

public enum DatabaseType { SqlServer, SQLite }

public class SqlExportService : IDisposable
{
    private DbConnection? _connection;
    private readonly DatabaseType _dbType;

    public SqlExportService(DatabaseType dbType) => _dbType = dbType;

    public async Task ConnectAsync(string connectionString)
    {
        _connection?.Dispose();
        _connection = _dbType switch
        {
            DatabaseType.SqlServer => new SqlConnection(connectionString),
            DatabaseType.SQLite    => new SqliteConnection(connectionString),
            _                      => throw new ArgumentOutOfRangeException(nameof(_dbType))
        };
        await _connection.OpenAsync();
    }

    public DatabaseType DbType => _dbType;

    public async Task<List<string>> GetDatabasesAsync()
    {
        EnsureConnected();
        if (_dbType != DatabaseType.SqlServer)
            throw new InvalidOperationException("Database listing is only supported for SQL Server.");

        const string sql = "SELECT name FROM sys.databases WHERE state = 0 ORDER BY name";
        var dbs = new List<string>();
        await using var cmd    = _connection!.CreateCommand();
        cmd.CommandText = sql;
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            dbs.Add(reader.GetString(0));

        return dbs;
    }

    public async Task<List<string>> GetTablesAsync()
    {
        EnsureConnected();
        var sql = _dbType switch
        {
            DatabaseType.SqlServer =>
                "SELECT TABLE_SCHEMA + '.' + TABLE_NAME " +
                "FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_TYPE = 'BASE TABLE' " +
                "ORDER BY TABLE_SCHEMA, TABLE_NAME",
            DatabaseType.SQLite =>
                "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name",
            _ => throw new ArgumentOutOfRangeException()
        };

        var tables = new List<string>();
        await using var cmd    = _connection!.CreateCommand();
        cmd.CommandText = sql;
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            tables.Add(reader.GetString(0));

        return tables;
    }

    public async Task<(DataField[] Fields, List<Dictionary<string, object?>> Rows)>
        GetTableDataAsync(string tableName, int? limit = null)
    {
        EnsureConnected();

        var safe = _dbType == DatabaseType.SqlServer
            ? QuoteSqlServerName(tableName)
            : $"\"{tableName.Replace("\"", "\"\"")}\"";

        var sql = (_dbType, limit) switch
        {
            (DatabaseType.SqlServer, int n) => $"SELECT TOP {n} * FROM {safe}",
            (DatabaseType.SQLite,    int n) => $"SELECT * FROM {safe} LIMIT {n}",
            _                               => $"SELECT * FROM {safe}"
        };

        await using var cmd    = _connection!.CreateCommand();
        cmd.CommandText = sql;
        await using var reader = await cmd.ExecuteReaderAsync();

        var fields = new DataField[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i);
            var clr  = MapToClr(reader.GetFieldType(i));
            fields[i] = new DataField(name, clr, isNullable: true);
        }

        var rows = new List<Dictionary<string, object?>>();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();
            for (int i = 0; i < reader.FieldCount; i++)
                row[fields[i].Name] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            rows.Add(row);
        }

        return (fields, rows);
    }

    private static string QuoteSqlServerName(string name)
    {
        var dot = name.IndexOf('.');
        if (dot >= 0)
        {
            var schema = name[..dot].Replace("]", "]]");
            var table  = name[(dot + 1)..].Replace("]", "]]");
            return $"[{schema}].[{table}]";
        }
        return $"[{name.Replace("]", "]]")}]";
    }

    private static Type MapToClr(Type sqlType)
    {
        if (sqlType == typeof(int)   || sqlType == typeof(short) || sqlType == typeof(byte)) return typeof(int);
        if (sqlType == typeof(long))    return typeof(long);
        if (sqlType == typeof(float))   return typeof(float);
        if (sqlType == typeof(double))  return typeof(double);
        if (sqlType == typeof(decimal)) return typeof(decimal);
        if (sqlType == typeof(bool))    return typeof(bool);
        if (sqlType == typeof(DateTime))return typeof(DateTime);
        if (sqlType == typeof(Guid))    return typeof(Guid);
        return typeof(string);
    }

    private void EnsureConnected()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
            throw new InvalidOperationException("Not connected to a database.");
    }

    public void Dispose() => _connection?.Dispose();
}
