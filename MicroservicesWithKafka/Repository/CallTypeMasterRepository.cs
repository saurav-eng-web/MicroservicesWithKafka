using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class CallTypeMasterRepository : ICallTypeMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public CallTypeMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<IEnumerable<CallTypeMaster>> GetAllAsync()
        {
            var list = new List<CallTypeMaster>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                using (var cmd = new SqlCommand("SP_CRUD_CallTypeMaster", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GETALL");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(MapToModel(reader));
                        }
                    }
                }
            }
            return list;
        }

        public async Task<CallTypeMaster?> GetByIdAsync(long id)
        {
            CallTypeMaster? result = null;
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                using (var cmd = new SqlCommand("SP_CRUD_CallTypeMaster", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GETBYID");
                    cmd.Parameters.AddWithValue("@CallType_ID", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result = MapToModel(reader);
                        }
                    }
                }
            }
            return result;
        }

        public async Task<long> AddAsync(CallTypeMaster callType)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                using (var cmd = new SqlCommand("SP_CRUD_CallTypeMaster", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@CallType_Name", (object?)callType.CallType_Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CallType_Active", (object?)callType.CallType_Active ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AP_EP", (object?)callType.AP_EP ?? DBNull.Value);

                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt64(result);
                }
            }
        }

        public async Task<long> UpdateAsync(CallTypeMaster callType)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                using (var cmd = new SqlCommand("SP_CRUD_CallTypeMaster", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "UPDATE");
                    cmd.Parameters.AddWithValue("@CallType_ID", callType.CallType_ID);
                    cmd.Parameters.AddWithValue("@CallType_Name", (object?)callType.CallType_Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CallType_Active", (object?)callType.CallType_Active ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AP_EP", (object?)callType.AP_EP ?? DBNull.Value);

                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt64(result);
                }
            }
        }

        public async Task<long> DeleteAsync(long id)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                using (var cmd = new SqlCommand("SP_CRUD_CallTypeMaster", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@CallType_ID", id);

                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt64(result);
                }
            }
        }

        private CallTypeMaster MapToModel(SqlDataReader reader)
        {
            return new CallTypeMaster
            {
                CallType_ID = Convert.ToInt64(reader["CallType_ID"]),
                CallType_Name = reader["CallType_Name"] as string,
                CallType_Active = reader.HasColumn("CallType_Active") ? reader["CallType_Active"] as string : null,
                AP_EP = reader.HasColumn("AP_EP") ? reader["AP_EP"] as string : null
            };
        }
    }

    // ✅ Helper extension for checking column existence
    public static class SqlDataReaderExtensions
    {
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
