using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class ASMMasterRepository : IASMMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ASMMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<ASMMaster> CreateASMMasterAsync(ASMMaster asm)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ASMMaster", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "CREATE");
            command.Parameters.AddWithValue("@ASM_Name", (object?)asm.ASM_Name ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", asm.IsActive);
            command.Parameters.AddWithValue("@EnteredBy", (object?)asm.EnteredBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)asm.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ContactNo", (object?)asm.ContactNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@DOJ", asm.DOJ.HasValue ? asm.DOJ.Value : DBNull.Value);


            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            asm.ASM_ID = id;
            return asm;
        }

        public async Task<ASMMaster?> GetASMMasterByIdAsync(int id)
        {
            ASMMaster? asm = null;
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ASMMaster", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "READ");
            command.Parameters.AddWithValue("@ASM_ID", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                asm = MapReader(reader);
            }

            return asm;
        }

        public async Task<List<ASMMaster>> GetAllASMMastersAsync()
        {
            var list = new List<ASMMaster>();
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ASMMaster", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "GETALL");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapReader(reader));
            }

            return list;
        }

        public async Task<List<ASMMaster>> GetAllRSMMastersAsync()
        {
            var list = new List<ASMMaster>();
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ASMMaster", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "getuserByRSM");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapReader(reader));
            }

            return list;
        }

        public async Task<bool> UpdateASMMasterAsync(ASMMaster asm)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ASMMaster", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "UPDATE");
            command.Parameters.AddWithValue("@ASM_ID", asm.ASM_ID);
            command.Parameters.AddWithValue("@ASM_Name", (object?)asm.ASM_Name ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", asm.IsActive);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)asm.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ContactNo", (object?)asm.ContactNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@DOJ", asm.DOJ.HasValue ? asm.DOJ.Value : DBNull.Value);


            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync(); // if it returns something → success
        }

        public async Task<bool> DeleteASMMasterAsync(int id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ASMMaster", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "DELETE");
            command.Parameters.AddWithValue("@ASM_ID", id);

            var result = await command.ExecuteScalarAsync();

            if (result != null && int.TryParse(result.ToString(), out int rows))
            {
                return rows > 0;
            }

            return false;
        }


        private ASMMaster MapReader(SqlDataReader reader)
        {
            return new ASMMaster
            {
                ASM_ID = Convert.ToInt32(reader["ASM_ID"]),
                ASM_Name = reader["ASM_Name"].ToString(),
                RSM_ID = Convert.ToInt32(reader["ASM_ID"]),
                RSM_Name = reader["RSM_Name"].ToString(),
                IsActive = Convert.ToInt32(reader["IsActive"]),
                EnteredDate = Convert.ToDateTime(reader["EnteredDate"]),
                EnteredBy = reader["EnteredBy"].ToString(),
                UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"]),
                UpdatedBy = reader["UpdatedBy"].ToString(),
                ContactNo = reader["ContactNo"].ToString(),
                DOJ = Convert.ToDateTime(reader["DOJ"])
            };
        }
    }
}
