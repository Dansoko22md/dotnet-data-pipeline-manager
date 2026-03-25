using System.Data;
using DPM.Core.Interfaces;
using DPM.Core.Models;
using Npgsql;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;

namespace DPM.Infrastructure.Readers
{
    public class DatabaseReader : IDataSourceReader
    {
        private readonly DataSource _source;

        public DatabaseReader(DataSource source)
        {
            _source = source;
        }

        public async Task<DataTable> ReadPreviewAsync(int maxRows = 50)
        {
            return await ReadAsync(maxRows);
        }

        public async Task<DataTable> ReadAllAsync()
        {
            return await ReadAsync(int.MaxValue);
        }

        private async Task<DataTable> ReadAsync(int maxRows)
        {
            var dt = new DataTable();
            var query = maxRows == int.MaxValue
                ? _source.Query!
                : $"SELECT * FROM ({_source.Query}) q LIMIT {maxRows}";

            switch (_source.Type)
            {
                case DataSourceType.PostgreSQL:
                    var pgConn = $"Host={_source.Host};Port={_source.Port};Database={_source.DatabaseName};Username={_source.Username};Password={_source.Password}";
                    await using (var conn = new NpgsqlConnection(pgConn))
                    {
                        await conn.OpenAsync();
                        await using var cmd = new NpgsqlCommand(query, conn);
                        await using var reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                    }
                    break;

                case DataSourceType.MySQL:
                    var myConn = $"Server={_source.Host};Port={_source.Port};Database={_source.DatabaseName};Uid={_source.Username};Pwd={_source.Password};";
                    using (var conn = new MySqlConnection(myConn))
                    {
                        await conn.OpenAsync();
                        using var cmd = new MySqlCommand(query, conn);
                        using var reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                    }
                    break;

                case DataSourceType.Oracle:
                    var orConn = $"Data Source={_source.Host}:{_source.Port}/{_source.DatabaseName};User Id={_source.Username};Password={_source.Password};";
                    using (var conn = new OracleConnection(orConn))
                    {
                        await conn.OpenAsync();
                        using var cmd = new OracleCommand(query, conn);
                        using var reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                    }
                    break;
            }

            return dt;
        }
    }
}