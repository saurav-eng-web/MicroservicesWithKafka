//using MicroservicesWithKafka.Models;
//using MicroservicesWithKafka.Services;
//using Microsoft.Data.SqlClient;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Threading.Tasks;

//namespace MicroservicesWithKafka.Repository
//{
//    public class CityMasterRepository : ICityMasterRepository
//    {
//        private readonly SQLConnectionService _sqlConnectionService;

//        public CityMasterRepository(SQLConnectionService sqlConnectionService)
//        {
//            _sqlConnectionService = sqlConnectionService;
//        }

//        private SqlCommand BuildCommand(SqlConnection connection, string action, CityMaster city)
//        {
//            var command = new SqlCommand("SP_CRUD_CityMaster", connection)
//            {
//                CommandType = CommandType.StoredProcedure
//            };

//            command.Parameters.AddWithValue("@Type", action);
//            command.Parameters.AddWithValue("@CINTCODE", (object?)city.CINTCODE ?? DBNull.Value);
//            command.Parameters.AddWithValue("@CITY", (object?)city.CITY ?? DBNull.Value);
//            command.Parameters.AddWithValue("@STDCODE", (object?)city.STDCODE ?? DBNull.Value);
//            command.Parameters.AddWithValue("@FTP", (object?)city.FTP ?? DBNull.Value);
//            command.Parameters.AddWithValue("@RegionID", (object?)city.RegionID ?? DBNull.Value);
//            command.Parameters.AddWithValue("@BranchID", (object?)city.BranchID ?? DBNull.Value);
//            command.Parameters.AddWithValue("@PCITY", (object?)city.PCITY ?? DBNull.Value);
//            command.Parameters.AddWithValue("@TTIME", (object?)city.TTIME ?? DBNull.Value);
//            command.Parameters.AddWithValue("@StateID", (object?)city.StateID ?? DBNull.Value);
//            command.Parameters.AddWithValue("@TSTATUS", (object?)city.TSTATUS ?? DBNull.Value);

//            return command;
//        }

//        public async Task<CityMaster> CreateCityAsync(CityMaster city)
//        {
//            using var connection = _sqlConnectionService.GetConnection();
//            await connection.OpenAsync();

//            using var command = BuildCommand(connection, "insert", city);
//            var result = await command.ExecuteScalarAsync();
//            city.CINTCODE = Convert.ToDecimal(result);

//            return city;
//        }

//        public async Task<bool> UpdateCityAsync(CityMaster city)
//        {
//            using var connection = _sqlConnectionService.GetConnection();
//            await connection.OpenAsync();

//            using var command = BuildCommand(connection, "update", city);
//            var result = await command.ExecuteScalarAsync();

//            return Convert.ToInt32(result) > 0;
//        }

//        public async Task<bool> DeleteCityAsync(decimal cityId)
//        {
//            using var connection = _sqlConnectionService.GetConnection();
//            await connection.OpenAsync();

//            using var command = new SqlCommand("SP_CRUD_CityMaster", connection)
//            {
//                CommandType = CommandType.StoredProcedure
//            };

//            command.Parameters.AddWithValue("@Type", "delete");
//            command.Parameters.AddWithValue("@CINTCODE", cityId);

//            var result = await command.ExecuteScalarAsync();
//            return Convert.ToInt32(result) > 0;
//        }

//        public async Task<CityMaster?> GetCityByIdAsync(decimal cityId)
//        {
//            using var connection = _sqlConnectionService.GetConnection();
//            await connection.OpenAsync();

//            using var command = new SqlCommand("SP_CRUD_CityMaster", connection)
//            {
//                CommandType = CommandType.StoredProcedure
//            };

//            command.Parameters.AddWithValue("@Type", "getbyid");
//            command.Parameters.AddWithValue("@CINTCODE", cityId);

//            using var reader = await command.ExecuteReaderAsync();
//            if (await reader.ReadAsync())
//            {
//                return MapToCity(reader);
//            }

//            return null;
//        }

//        public async Task<List<CityMaster>> GetAllCitiesAsync()
//        {
//            var cities = new List<CityMaster>();

//            using var connection = _sqlConnectionService.GetConnection();
//            await connection.OpenAsync();

//            using var command = new SqlCommand("SP_CRUD_CityMaster", connection)
//            {
//                CommandType = CommandType.StoredProcedure
//            };

//            command.Parameters.AddWithValue("@Type", "get");

//            using var reader = await command.ExecuteReaderAsync();
//            while (await reader.ReadAsync())
//            {
//                cities.Add(MapToCity(reader));
//            }

//            return cities;
//        }

//        private CityMaster MapToCity(SqlDataReader reader)
//        {
//            return new CityMaster
//            {
//                CINTCODE = reader["CINTCODE"] as decimal?,
//                CITY = reader["CITY"]?.ToString(),
//                STDCODE = reader["STDCODE"]?.ToString(),
//                FTP = reader["FTP"]?.ToString()?[0],
//                RegionID = reader["RegionID"] as decimal?,
//                BranchID = reader["BranchID"] as decimal?,
//                PCITY = reader["PCITY"] as decimal?,
//                TDATE = reader["TDATE"] as DateTime?,
//                TTIME = reader["TTIME"] as DateTime?,
//                StateID = reader["StateID"] as decimal?,
//                TSTATUS = reader["TSTATUS"]?.ToString()?[0]
//            };
//        }
//    }
//}



using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class CityMasterRepository : ICityMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public CityMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private SqlCommand BuildCommand(SqlConnection connection, string action, CityMaster city)
        {
            var command = new SqlCommand("SP_CRUD_CityMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", action);
            command.Parameters.AddWithValue("@CINTCODE", (object?)city.CINTCODE ?? DBNull.Value);
            command.Parameters.AddWithValue("@CITY", (object?)city.CITY ?? DBNull.Value);
            command.Parameters.AddWithValue("@STDCODE", (object?)city.STDCODE ?? DBNull.Value);
            command.Parameters.AddWithValue("@FTP", (object?)city.FTP ?? DBNull.Value);
            command.Parameters.AddWithValue("@RegionID", (object?)city.RegionID ?? DBNull.Value);
            command.Parameters.AddWithValue("@BranchID", (object?)city.BranchID ?? DBNull.Value);
            command.Parameters.AddWithValue("@PCITY", (object?)city.PCITY ?? DBNull.Value);
            command.Parameters.AddWithValue("@TTIME", (object?)city.TTIME ?? DBNull.Value);
            command.Parameters.AddWithValue("@StateID", (object?)city.StateID ?? DBNull.Value);
            command.Parameters.AddWithValue("@TSTATUS", (object?)city.TSTATUS ?? DBNull.Value);

            return command;
        }

        public async Task<CityMaster> CreateCityAsync(CityMaster city)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "insert", city);
            var result = await command.ExecuteScalarAsync();
            city.CINTCODE = Convert.ToDecimal(result);

            return city;
        }

        public async Task<bool> UpdateCityAsync(CityMaster city)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "update", city);
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteCityAsync(decimal cityId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_CityMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "delete");
            command.Parameters.AddWithValue("@CINTCODE", cityId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<CityMaster?> GetCityByIdAsync(decimal cityId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_CityMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "getbyid");
            command.Parameters.AddWithValue("@CINTCODE", cityId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToCity(reader);
            }

            return null;
        }

        public async Task<List<CityMaster>> GetAllCitiesAsync()
        {
            var cities = new List<CityMaster>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_CityMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "get");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                cities.Add(MapToCity(reader));
            }

            return cities;
        }

        private CityMaster MapToCity(SqlDataReader reader)
        {
            return new CityMaster
            {
                CINTCODE = reader["CINTCODE"] != DBNull.Value ? (decimal?)reader["CINTCODE"] : null,
                CITY = reader["CITY"]?.ToString(),
                STDCODE = reader["STDCODE"]?.ToString(),
                FTP = reader["FTP"] != DBNull.Value && !string.IsNullOrEmpty(reader["FTP"].ToString())
                    ? reader["FTP"].ToString()[0]
                    : (char?)null,
                RegionID = reader["RegionID"] != DBNull.Value ? (decimal?)reader["RegionID"] : null,
                BranchID = reader["BranchID"] != DBNull.Value ? (decimal?)reader["BranchID"] : null,
                PCITY = reader["PCITY"] != DBNull.Value ? (decimal?)reader["PCITY"] : null,
                TDATE = reader["TDATE"] != DBNull.Value ? (DateTime?)reader["TDATE"] : null,
                TTIME = reader["TTIME"] != DBNull.Value ? (DateTime?)reader["TTIME"] : null,
                StateID = reader["StateID"] != DBNull.Value ? (decimal?)reader["StateID"] : null,
                TSTATUS = reader["TSTATUS"] != DBNull.Value && !string.IsNullOrEmpty(reader["TSTATUS"].ToString())
                    ? reader["TSTATUS"].ToString()[0]
                    : (char?)null
            };
        }
    }
}
