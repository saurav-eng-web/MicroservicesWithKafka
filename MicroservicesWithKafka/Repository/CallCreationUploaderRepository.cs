using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class CallCreationUploaderRepository : ICallCreationUploaderRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public CallCreationUploaderRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<List<int>> CreateBulkAsync(List<CallCreationUploader> models)
        {
            var insertedIds = new List<int>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            foreach (var model in models)
            {
                using var cmd = new SqlCommand("SP_CRUD_Tbl_CallCreationUploader_CRUD", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameters(cmd, model, "INSERT");

                var result = await cmd.ExecuteScalarAsync();
                insertedIds.Add(Convert.ToInt32(result));
            }

            return insertedIds;
        }

        public async Task<string> UpdateAsync(CallCreationUploader model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Tbl_CallCreationUploader_CRUD", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            AddParameters(cmd, model, "UPDATE");

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "Updated";
        }

        public async Task<string> DeleteAsync(int id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Tbl_CallCreationUploader_CRUD", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "DELETE");
            cmd.Parameters.AddWithValue("@Id", id);

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "Deleted";
        }

        public async Task<IEnumerable<CallCreationUploader>> GetAllAsync()
        {
            var list = new List<CallCreationUploader>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Tbl_CallCreationUploader_CRUD", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SELECT");

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapToModel(reader));
            }

            return list;
        }

        public async Task<CallCreationUploader?> GetByIdAsync(int id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Tbl_CallCreationUploader_CRUD", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToModel(reader);
            }
            return null;
        }

        private static void AddParameters(SqlCommand cmd, CallCreationUploader model, string action)
        {
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@First_Name", model.First_Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Last_Name", model.Last_Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MobileVal", model.MobileVal ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Mobile", model.Mobile ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Alternate_Phone", model.Alternate_Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Permanent_Address", model.Permanent_Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Landmark", model.Landmark ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PincodeVal", model.PincodeVal ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Pincode", model.Pincode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Title", model.Title ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Serial_No", model.Serial_No ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Model", model.Model ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ModelVal", model.ModelVal ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CallBy", model.CallBy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CallType", model.CallType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Purchase_Date", model.Purchase_Date ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedDateTime", model.CreatedDateTime ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Isprocess1", model.Isprocess1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Isprocess", model.Isprocess ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IpAddress", model.IpAddress ?? (object)DBNull.Value);
        }

        private static CallCreationUploader MapToModel(SqlDataReader reader)
        {
            return new CallCreationUploader
            {
                Id = Convert.ToInt32(reader["Id"]),
                First_Name = reader["First_Name"]?.ToString(),
                Last_Name = reader["Last_Name"]?.ToString(),
                MobileVal = reader["MobileVal"] as bool? ?? false,
                Mobile = reader["Mobile"]?.ToString(),
                Alternate_Phone = reader["Alternate_Phone"]?.ToString(),
                Permanent_Address = reader["Permanent_Address"]?.ToString(),
                Landmark = reader["Landmark"]?.ToString(),
                PincodeVal = reader["PincodeVal"] as bool? ?? false,
                Pincode = reader["Pincode"] != DBNull.Value ? Convert.ToInt64(reader["Pincode"]) : 0,
                Title = reader["Title"]?.ToString(),
                Serial_No = reader["Serial_No"]?.ToString(),
                Model = reader["Model"]?.ToString(),
                ModelVal = reader["ModelVal"] as int?,
                CallBy = reader["CallBy"]?.ToString(),
                CallType = reader["CallType"]?.ToString(),
                Purchase_Date = reader["Purchase_Date"] as DateTime?,
                CreatedDateTime = reader["CreatedDateTime"] as DateTime?,
                Isprocess1 = reader["Isprocess1"] as bool?,
                Isprocess = reader["Isprocess"] as bool?,
                CreatedBy = reader["CreatedBy"]?.ToString(),
                IpAddress = reader["IpAddress"]?.ToString()
            };
        }
    }
}
