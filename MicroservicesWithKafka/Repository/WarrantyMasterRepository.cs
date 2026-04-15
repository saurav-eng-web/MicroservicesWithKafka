using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class WarrantyMasterRepository : IWarrantyMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public WarrantyMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<IEnumerable<WarrantyMaster>> GetAllAsync()
        {
            var warranties = new List<WarrantyMaster>();
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_WarrantyMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Action", "SELECT_ALL");

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                warranties.Add(MapToModel(reader));
            }
            return warranties;
        }

        public async Task<WarrantyMaster?> GetByIdAsync(long id)
        {
            WarrantyMaster? warranty = null;
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_WarrantyMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
            cmd.Parameters.AddWithValue("@Warranty_ID", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                warranty = MapToModel(reader);
            }
            return warranty;
        }

        public async Task<long> AddAsync(WarrantyMaster warranty)
        {
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_WarrantyMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "INSERT");
            cmd.Parameters.AddWithValue("@Warranty_Desc", warranty.Warranty_Desc ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Warranty_Active", warranty.Warranty_Active ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Warranty_Type", warranty.Warranty_Type ?? (object)DBNull.Value);

            var outputId = new SqlParameter("@NewId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outputId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return Convert.ToInt64(outputId.Value);
        }

        public async Task<long> UpdateAsync(WarrantyMaster warranty)
        {
            if (warranty.Warranty_ID == null)
                throw new ArgumentException("Warranty_ID cannot be null for update.");

            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_WarrantyMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "UPDATE");
            cmd.Parameters.AddWithValue("@Warranty_ID", warranty.Warranty_ID.Value); // safe since not null
            cmd.Parameters.AddWithValue("@Warranty_Desc", warranty.Warranty_Desc ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Warranty_Active", warranty.Warranty_Active ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Warranty_Type", warranty.Warranty_Type ?? (object)DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return warranty.Warranty_ID.Value;
        }


        public async Task<long> DeleteAsync(long id)
        {
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_WarrantyMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "DELETE");
            cmd.Parameters.AddWithValue("@Warranty_ID", id);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return id;
        }

        private WarrantyMaster MapToModel(SqlDataReader reader)
        {
            return new WarrantyMaster
            {
                Warranty_ID = Convert.ToInt64(reader["Warranty_ID"]),
                Warranty_Desc = reader["Warranty_Desc"] as string,
                Warranty_Active = reader["Warranty_Active"] as string,
                Warranty_Type = reader["Warranty_Type"] as string
            };
        }
    }
}
