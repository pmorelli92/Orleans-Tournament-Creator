using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using Npgsql;

namespace Snaelro.Projections
{
    public class ProjectionManager<T>
    {
        private readonly string _getQuery;
        private readonly string _getAllQuery;
        private readonly string _updateCommand;
        private readonly PostgresOptions _postgresOptions;

        public ProjectionManager(
            string schemaName,
            string tableName,
            PostgresOptions postgresOptions)
        {
            _postgresOptions = postgresOptions;

            _getQuery = $"SELECT payload FROM {schemaName}.{tableName} WHERE id = @id";
            _getAllQuery = $"SELECT payload FROM {schemaName}.{tableName}";
            _updateCommand = $"INSERT INTO {schemaName}.{tableName} (id, payload) VALUES (@id, @payload::jsonb) ON CONFLICT(id) DO UPDATE SET payload = excluded.payload;";
        }

        public async Task<T> GetProjectionAsync(Guid id)
        {
            using (var connection = new NpgsqlConnection(_postgresOptions.ConnectionString))
            {
                var payload = await connection.QuerySingleOrDefaultAsync<string>(_getQuery, param: new { id });

                return payload is null
                    ? default
                    : JsonConvert.DeserializeObject<T>(payload);
            }
        }

        public async Task<IReadOnlyList<T>> GetProjectionsAsync()
        {
            using (var connection = new NpgsqlConnection(_postgresOptions.ConnectionString))
            {
                var payloads = await connection.QueryAsync<string>(_getAllQuery);
                var array = $"[{string.Join(",", payloads)}]";
                return JsonConvert.DeserializeObject<IReadOnlyList<T>>(array);
            }
        }

        public async Task UpdateProjection(Guid id, T projection)
        {
            using (var connection = new NpgsqlConnection(_postgresOptions.ConnectionString))
                await connection.ExecuteAsync(_updateCommand, param: new { id, payload = JsonConvert.SerializeObject(projection) });
        }
    }
}