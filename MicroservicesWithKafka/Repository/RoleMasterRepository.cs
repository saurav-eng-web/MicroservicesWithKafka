using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class RoleMasterRepository : IRoleMasterRepository
    {
        private readonly SQLConnectionService _connectionService;

        public RoleMasterRepository(SQLConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        private SqlCommand BuildCommand(SqlConnection connection, string action, RoleMaster model)
        {
            var cmd = new SqlCommand("SP_CRUD_RoleMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", action);
            cmd.Parameters.AddWithValue("@RM_ID", (object?)model.RM_ID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RM_Name", (object?)model.RM_Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RM_Status", (object?)model.RM_Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AndroidActive", (object?)model.AndroidActive ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsB2B", (object?)model.IsB2B ?? DBNull.Value);

            return cmd;
        }

        public async Task<RoleMaster?> GetRoleByIdAsync(int id)
        {
            using var connection = _connectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_RoleMaster", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Type", "getbyid");
            cmd.Parameters.AddWithValue("@RM_ID", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToModel(reader);
            }

            return null;
        }

        public async Task<List<RoleMaster>> GetAllRolesAsync()
        {
            var list = new List<RoleMaster>();

            using var connection = _connectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_RoleMaster", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Type", "get");

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapToModel(reader));
            }

            return list;
        }

        public async Task<RoleMaster> CreateRoleAsync(RoleMaster model)
        {
            using var connection = _connectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = BuildCommand(connection, "insert", model);
            var result = await cmd.ExecuteScalarAsync();
            model.RM_ID = Convert.ToInt32(result);

            return model;
        }

        public async Task<bool> UpdateRoleAsync(RoleMaster model)
        {
            using var connection = _connectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = BuildCommand(connection, "update", model);
            var result = await cmd.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            using var connection = _connectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_RoleMaster", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Type", "delete");
            cmd.Parameters.AddWithValue("@RM_ID", id);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        private RoleMaster MapToModel(SqlDataReader reader)
        {
            return new RoleMaster
            {
                RM_ID = reader["RM_ID"] as int?,
                RM_Name = reader["RM_Name"]?.ToString(),
                RM_Status = reader["RM_Status"] as bool?,
                AndroidActive = reader["AndroidActive"] as bool?,
                IsB2B = reader["IsB2B"] as bool?
            };
        }
    }

}
