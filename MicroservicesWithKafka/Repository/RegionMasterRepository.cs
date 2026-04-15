using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class RegionMasterRepository : IRegionMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public RegionMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private SqlCommand BuildCommand(SqlConnection connection, string action, RegionMaster region)
        {
            var command = new SqlCommand("SP_CRUD_RegionMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", action);
            command.Parameters.AddWithValue("@Rid", (object?)region.Rid ?? DBNull.Value);
            command.Parameters.AddWithValue("@Regionname", (object?)region.Regionname ?? DBNull.Value);
            command.Parameters.AddWithValue("@tdate", (object?)region.Tdate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ttime", (object?)region.Ttime ?? DBNull.Value);
            command.Parameters.AddWithValue("@status", (object?)region.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@SN", (object?)region.SN ?? DBNull.Value);

            return command;
        }

        public async Task<RegionMaster> CreateRegionAsync(RegionMaster region)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "insert", region);
            var result = await command.ExecuteScalarAsync();
            region.Rid = Convert.ToDecimal(result);

            return region;
        }

        public async Task<bool> UpdateRegionAsync(RegionMaster region)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "update", region);
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteRegionAsync(decimal regionId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_RegionMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "delete");
            command.Parameters.AddWithValue("@Rid", regionId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<RegionMaster?> GetRegionByIdAsync(decimal regionId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_RegionMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "getbyid");
            command.Parameters.AddWithValue("@Rid", regionId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToRegion(reader);
            }

            return null;
        }

        public async Task<List<RegionMaster>> GetAllRegionsAsync()
        {
            var regions = new List<RegionMaster>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_RegionMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "get");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                regions.Add(MapToRegion(reader));
            }

            return regions;
        }

        private RegionMaster MapToRegion(SqlDataReader reader)
        {
            return new RegionMaster
            {
                Rid = reader["Rid"] as decimal?,
                Regionname = reader["Regionname"]?.ToString(),
                Tdate = reader["tdate"] as DateTime?,
                Ttime = reader["ttime"] as DateTime?,
                Status = reader["status"]?.ToString(),
                SN = reader["SN"]?.ToString()
            };
        }
    }
}
