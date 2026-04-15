using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class SymptomMasterRepository : ISymptomMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public SymptomMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<IEnumerable<SymptomMaster>> GetAllAsync()
        {
            var symptoms = new List<SymptomMaster>();
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_SymptomMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Action", "SELECT_ALL");
            cmd.Parameters.Add("@NewId", SqlDbType.BigInt).Direction = ParameterDirection.Output;

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                symptoms.Add(MapToModel(reader));
            }
            return symptoms;
        }

        public async Task<SymptomMaster?> GetByIdAsync(long id)
        {
            SymptomMaster? symptom = null;
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_SymptomMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
            cmd.Parameters.AddWithValue("@syid", id);
            cmd.Parameters.Add("@NewId", SqlDbType.BigInt).Direction = ParameterDirection.Output;

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                symptom = MapToModel(reader);
            }
            return symptom;
        }

        public async Task<long> AddAsync(SymptomMaster symptom)
        {
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_SymptomMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "INSERT");
            cmd.Parameters.AddWithValue("@SYproduct", symptom.SYproduct ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@symptom", symptom.symptom ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", symptom.status ?? (object)DBNull.Value);

            var outputId = new SqlParameter("@NewId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outputId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return Convert.ToInt64(outputId.Value);
        }

        public async Task<long> UpdateAsync(SymptomMaster symptom)
        {
            if (symptom.syid == null)
                throw new ArgumentException("syid cannot be null for update.");

            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_SymptomMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "UPDATE");
            cmd.Parameters.AddWithValue("@syid", symptom.syid.Value);
            cmd.Parameters.AddWithValue("@SYproduct", symptom.SYproduct ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@symptom", symptom.symptom ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", symptom.status ?? (object)DBNull.Value);

            cmd.Parameters.Add("@NewId", SqlDbType.BigInt).Direction = ParameterDirection.Output;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return symptom.syid.Value;
        }

        public async Task<long> DeleteAsync(long id)
        {
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_SymptomMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "DELETE");
            cmd.Parameters.AddWithValue("@syid", id);
            cmd.Parameters.Add("@NewId", SqlDbType.BigInt).Direction = ParameterDirection.Output;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return id;
        }

        private SymptomMaster MapToModel(SqlDataReader reader)
        {
            return new SymptomMaster
            {
                syid = Convert.ToInt64(reader["syid"]),
                SYproduct = reader["SYproduct"] as string,
                symptom = reader["symptom"] as string,
                status = reader["status"] as string
            };
        }

        public async Task<IEnumerable<SymptomMaster>> GetByProductIdAsync(long productId)
        {
            var symptoms = new List<SymptomMaster>();
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_SymptomMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "GetSystomByPid");
            cmd.Parameters.AddWithValue("@productid", productId);
            cmd.Parameters.Add("@NewId", SqlDbType.BigInt).Direction = ParameterDirection.Output;

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                symptoms.Add(new SymptomMaster
                {
                    syid = Convert.ToInt64(reader["syid"]),
                    symptom = reader["symptom"] as string
                    // only these 2 columns selected in SP
                });
            }
            return symptoms;
        }

    }
}
