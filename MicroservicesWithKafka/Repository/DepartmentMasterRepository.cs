using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class DepartmentMasterRepository : IDepartmentMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public DepartmentMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private SqlCommand BuildCommand(SqlConnection connection, string action, DepartmentMaster department)
        {
            var command = new SqlCommand("SP_CRUD_DepartmentMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", action);
            command.Parameters.AddWithValue("@DepartmentID", (object?)department.DepartmentID ?? DBNull.Value);
            command.Parameters.AddWithValue("@DepartmentName", (object?)department.DepartmentName ?? DBNull.Value);
            command.Parameters.AddWithValue("@DepartmentCode", (object?)department.DepartmentCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)department.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", department.Status);

            return command;
        }

        public async Task<bool> CreateAsync(DepartmentMaster department)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "INSERT", department);
            var result = await command.ExecuteScalarAsync();

            department.DepartmentID = result != null ? Convert.ToInt64(result) : 0;
            return department.DepartmentID > 0;
        }

        public async Task<bool> UpdateAsync(DepartmentMaster department)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "UPDATE", department);
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_DepartmentMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "DELETE");
            command.Parameters.AddWithValue("@DepartmentID", id);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<DepartmentMaster?> GetByIdAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_DepartmentMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "GETBYID");
            command.Parameters.AddWithValue("@DepartmentID", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToDepartment(reader);
            }

            return null;
        }

        public async Task<IEnumerable<DepartmentMaster>> GetAllAsync()
        {
            var departments = new List<DepartmentMaster>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_DepartmentMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "GETALL");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                departments.Add(MapToDepartment(reader));
            }

            return departments;
        }

        private DepartmentMaster MapToDepartment(SqlDataReader reader)
        {
            return new DepartmentMaster
            {
                DepartmentID = reader["DepartmentID"] != DBNull.Value ? Convert.ToInt64(reader["DepartmentID"]) : null,
                DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty,
                DepartmentCode = reader["DepartmentCode"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString(),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                CreatedDateTime = reader["CreatedDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDateTime"]) : null,
                UpdatedDateTime = reader["UpdatedDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["UpdatedDateTime"]) : null
            };
        }
    }
}
