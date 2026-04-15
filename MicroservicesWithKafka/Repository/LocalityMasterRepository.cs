using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class LocalityMasterRepository : ILocalityMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LocalityMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private SqlCommand BuildCommand(SqlConnection connection, string action, LocalityMaster model)
        {
            var cmd = new SqlCommand("SP_CRUD_LocalityMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", action);
            cmd.Parameters.AddWithValue("@Locality_Id", (object?)model.Locality_Id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Locality_RegionID", (object?)model.Locality_RegionID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Locality_BranchID", (object?)model.Locality_BranchID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Locality_City", (object?)model.Locality_City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Locality_Name", (object?)model.Locality_Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Pincode", (object?)model.Pincode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Locality_Status", (object?)model.Locality_Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@locality_c_u", (object?)model.locality_c_u ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Locality_State", (object?)model.Locality_State ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ResolutionTATID", (object?)model.ResolutionTATID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Distance", (object?)model.Distance ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Installation_Demo_TAT", (object?)model.Installation_Demo_TAT ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ServiceCall_Visit_TAT", (object?)model.ServiceCall_Visit_TAT ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ServiceCall_Repair_TAT", (object?)model.ServiceCall_Repair_TAT ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Installation_Demo_TAT_Num", (object?)model.Installation_Demo_TAT_Num ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ServiceCall_Repair_TAT_Num", (object?)model.ServiceCall_Repair_TAT_Num ?? DBNull.Value);

            return cmd;
        }

        public async Task<LocalityMaster> CreateLocalityAsync(LocalityMaster model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = BuildCommand(connection, "insert", model);
            var result = await cmd.ExecuteScalarAsync();
            model.Locality_Id = Convert.ToInt64(result);

            return model;
        }

        public async Task<bool> UpdateLocalityAsync(LocalityMaster model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = BuildCommand(connection, "update", model);
            var result = await cmd.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteLocalityAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_LocalityMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "delete");
            cmd.Parameters.AddWithValue("@Locality_Id", id);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<List<LocalityMaster>> GetLocalitiesByPincodeAsync(string pincode)
        {
            var list = new List<LocalityMaster>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_LocalityMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "getbypincode");
            cmd.Parameters.AddWithValue("@Pincode", pincode);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapToModel(reader));
            }

            return list;
        }

        public async Task<LocalityMaster?> GetLocalityByIdAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_LocalityMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "getbyid");
            cmd.Parameters.AddWithValue("@Locality_Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToModel(reader);
            }

            return null;
        }

        public async Task<List<LocalityMaster>> GetAllLocalitiesAsync()
        {
            var list = new List<LocalityMaster>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_LocalityMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "get");

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapToModel(reader));
            }

            return list;
        }

        private LocalityMaster MapToModel(SqlDataReader reader)
        {
            return new LocalityMaster
            {
                Locality_Id = reader["Locality_Id"] == DBNull.Value ? null : Convert.ToDecimal(reader["Locality_Id"]),
                Locality_RegionID = reader["Locality_RegionID"] == DBNull.Value ? null : Convert.ToDecimal(reader["Locality_RegionID"]),
                Locality_BranchID = reader["Locality_BranchID"] == DBNull.Value ? null : Convert.ToDecimal(reader["Locality_BranchID"]),
                Locality_City = reader["Locality_City"] == DBNull.Value ? null : Convert.ToDecimal(reader["Locality_City"]),
                Locality_Name = reader["Locality_Name"] == DBNull.Value ? null : reader["Locality_Name"].ToString(),
                Pincode = reader["Pincode"] == DBNull.Value ? null : reader["Pincode"].ToString(),
                Locality_Status = reader["Locality_Status"] == DBNull.Value ? null : Convert.ToBoolean(reader["Locality_Status"]),
                locality_c_u = reader["locality_c_u"] == DBNull.Value ? null : reader["locality_c_u"].ToString(),
                Locality_State = reader["Locality_State"] == DBNull.Value ? null : Convert.ToDecimal(reader["Locality_State"]),
                ResolutionTATID = reader["ResolutionTATID"] == DBNull.Value ? null : Convert.ToDecimal(reader["ResolutionTATID"]),
                Distance = reader["Distance"] == DBNull.Value ? null : Convert.ToDecimal(reader["Distance"]),
                Installation_Demo_TAT = reader["Installation_Demo_TAT"] == DBNull.Value ? null : reader["Installation_Demo_TAT"].ToString(),
                ServiceCall_Visit_TAT = reader["ServiceCall_Visit_TAT"] == DBNull.Value ? null : reader["ServiceCall_Visit_TAT"].ToString(),
                ServiceCall_Repair_TAT = reader["ServiceCall_Repair_TAT"] == DBNull.Value ? null : reader["ServiceCall_Repair_TAT"].ToString(),
                Installation_Demo_TAT_Num = reader["Installation_Demo_TAT_Num"] == DBNull.Value ? null : Convert.ToDecimal(reader["Installation_Demo_TAT_Num"]),
                ServiceCall_Repair_TAT_Num = reader["ServiceCall_Repair_TAT_Num"] == DBNull.Value ? null : Convert.ToDecimal(reader["ServiceCall_Repair_TAT_Num"]),
                State_Name = reader["State_Name"] == DBNull.Value ? null : reader["State_Name"].ToString(),
                City_Name = reader["City_Name"] == DBNull.Value ? null : reader["City_Name"].ToString()

            };
        }
    }
}
