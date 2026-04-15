
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class ASC_MASTERRepository : IASC_MASTERRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ASC_MASTERRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        // INSERT
        public async Task<ASC_MASTER> CreateAscAsync(ASC_MASTER asc)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASC_MASTER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "insert");

                    AddAscParameters(command, asc);

                    var result = await command.ExecuteScalarAsync();
                    asc.ASCINTCODE = Convert.ToInt64(result);
                }
            }
            return asc;
        }

        // UPDATE
        public async Task<bool> UpdateAscAsync(ASC_MASTER asc)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASC_MASTER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "update");
                    command.Parameters.AddWithValue("@ASCINTCODE", asc.ASCINTCODE);

                    AddAscParameters(command, asc);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                            return rows > 0;
                        }
                    }
                }
            }
            return false;
        }

        // DELETE
        public async Task<bool> DeleteAscAsync(long ascId)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASC_MASTER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "delete");
                    command.Parameters.AddWithValue("@ASCINTCODE", ascId);

                    // Add dummy values to avoid missing parameter issues
                    AddAscParameters(command, new ASC_MASTER());

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                            return rows > 0;
                        }
                    }
                }
            }
            return false;
        }

        // GET BY ID
        public async Task<ASC_MASTER?> GetAscByIdAsync(long ascId)
        {
            ASC_MASTER? asc = null;

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASC_MASTER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "getbyid");
                    command.Parameters.AddWithValue("@ASCINTCODE", ascId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            asc = MapAsc(reader);
                        }
                    }
                }
            }
            return asc;
        }

        // GET BY asccode
        public async Task<ASC_MASTER?> GetASCByasccodeAsync(string asccode)
        {
            ASC_MASTER? asc = null;

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASC_MASTER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "getbyasccode");
                    command.Parameters.AddWithValue("@ASC_CODE", asccode);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            asc = MapAsc(reader);
                        }
                    }
                }
            }
            return asc;
        }



        public async Task<List<Dictionary<string, object>>> GetAscByRoleAsync(string role,int id)
        {
            var list = new List<Dictionary<string, object>>();

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASC_MASTER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "getbyrole");
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }

                            list.Add(row);
                        }
                    }
                }
            }
            return list;
        }

        // GET ALL
        public async Task<List<ASC_MASTER>> GetAllAscAsync()
        {
            var list = new List<ASC_MASTER>();

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASC_MASTER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "get");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(MapAsc(reader));
                        }
                    }
                }
            }
            return list;
        }

        // 🔹 Helper: Add all parameters
        private void AddAscParameters(SqlCommand command, ASC_MASTER asc)
        {
            command.Parameters.AddWithValue("@ASC_CODE", (object?)asc.ASC_CODE ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASC_NAME", (object?)asc.ASC_NAME ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASC_PATH", (object?)asc.ASC_PATH ?? DBNull.Value);
            command.Parameters.AddWithValue("@TDATE", (object?)asc.TDATE ?? DBNull.Value);
            command.Parameters.AddWithValue("@TTIME", (object?)asc.TTIME ?? DBNull.Value);
            command.Parameters.AddWithValue("@ENTERBY", (object?)asc.ENTERBY ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASC_STATUS", (object?)asc.ASC_STATUS ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASC_CatID", (object?)asc.ASC_CatID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)asc.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)asc.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Region", (object?)asc.Region ?? DBNull.Value);
            command.Parameters.AddWithValue("@Branch", (object?)asc.Branch ?? DBNull.Value);
            command.Parameters.AddWithValue("@City", (object?)asc.City ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address", (object?)asc.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@MobileNo", (object?)asc.MobileNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@SAC", (object?)asc.SAC ?? DBNull.Value);
            command.Parameters.AddWithValue("@SHA", (object?)asc.SHA ?? DBNull.Value);
            command.Parameters.AddWithValue("@SCE", (object?)asc.SCE ?? DBNull.Value);
            command.Parameters.AddWithValue("@SSD", (object?)asc.SSD ?? DBNull.Value);
            command.Parameters.AddWithValue("@FTP", (object?)asc.FTP ?? DBNull.Value);
            command.Parameters.AddWithValue("@Asc_Category", (object?)asc.Asc_Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASC_Category1", (object?)asc.ASC_Category1 ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASC_Category2", (object?)asc.ASC_Category2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@CatID1", (object?)asc.CatID1 ?? DBNull.Value);
            command.Parameters.AddWithValue("@CatID2", (object?)asc.CatID2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@CatID3", (object?)asc.CatID3 ?? DBNull.Value);
            command.Parameters.AddWithValue("@Pan_Number", (object?)asc.Pan_Number ?? DBNull.Value);
            command.Parameters.AddWithValue("@ST_Number", (object?)asc.ST_Number ?? DBNull.Value);
            command.Parameters.AddWithValue("@DOJ", (object?)asc.DOJ ?? DBNull.Value);
            command.Parameters.AddWithValue("@SAPCode", (object?)asc.SAPCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@RegionId", (object?)asc.RegionId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Bid", (object?)asc.Bid ?? DBNull.Value);
            command.Parameters.AddWithValue("@CotPerson", (object?)asc.CotPerson ?? DBNull.Value);
            command.Parameters.AddWithValue("@CityId", (object?)asc.CityId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Locality", (object?)asc.Locality ?? DBNull.Value);
            command.Parameters.AddWithValue("@SendSms", (object?)asc.SendSms ?? DBNull.Value);
            command.Parameters.AddWithValue("@MinGaurantee", (object?)asc.MinGaurantee ?? DBNull.Value);
            command.Parameters.AddWithValue("@SecurityAmount", (object?)asc.SecurityAmount ?? DBNull.Value);
            command.Parameters.AddWithValue("@AvgCallLoad", (object?)asc.AvgCallLoad ?? DBNull.Value);
            command.Parameters.AddWithValue("@BankName", (object?)asc.BankName ?? DBNull.Value);
            command.Parameters.AddWithValue("@AccountNo", (object?)asc.AccountNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@PORecieveByAdmin", (object?)asc.PORecieveByAdmin ?? DBNull.Value);
            command.Parameters.AddWithValue("@TinNo", (object?)asc.TinNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@NAVCode", (object?)asc.NAVCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Pincode", (object?)asc.Pincode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Latitude", (object?)asc.Latitude ?? DBNull.Value);
            command.Parameters.AddWithValue("@Longitude", (object?)asc.Longitude ?? DBNull.Value);
            command.Parameters.AddWithValue("@StateID", (object?)asc.StateID ?? DBNull.Value);
            command.Parameters.AddWithValue("@AccountHolderName", (object?)asc.AccountHolderName ?? DBNull.Value);
            command.Parameters.AddWithValue("@BankBranch", (object?)asc.BankBranch ?? DBNull.Value);
            command.Parameters.AddWithValue("@IFSCCode", (object?)asc.IFSCCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASM_ID", (object?)asc.ASM_ID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASC_GST", (object?)asc.ASC_GST ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASC_State", (object?)asc.ASC_State ?? DBNull.Value);
            command.Parameters.AddWithValue("@ASC_StateCode", (object?)asc.ASC_StateCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@BranchWarehouse", (object?)asc.BranchWarehouse ?? DBNull.Value);
            command.Parameters.AddWithValue("@CorrectionWarehouse", (object?)asc.CorrectionWarehouse ?? DBNull.Value);
        }

        // 🔹 Helper: Map reader to model
        private ASC_MASTER MapAsc(SqlDataReader reader)
        {
            return new ASC_MASTER
            {
                ASCINTCODE = reader.GetSchemaTable()?.Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "ASCINTCODE") == true && reader["ASCINTCODE"] != DBNull.Value? Convert.ToInt64(reader["ASCINTCODE"]): 0,

                ASC_CODE = reader["ASC_CODE"] == DBNull.Value ? null : reader["ASC_CODE"].ToString(),
                ASC_NAME = reader["ASC_NAME"] == DBNull.Value ? null : reader["ASC_NAME"].ToString(),
                ASC_PATH = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "ASC_PATH")? reader["ASC_PATH"]?.ToString(): null,

                ENTERBY = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "ENTERBY")? reader["ENTERBY"]?.ToString(): null,

                ASC_STATUS = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "ASC_STATUS")? reader["ASC_STATUS"]?.ToString(): null,

                Phone = reader["Phone"] == DBNull.Value ? null : reader["Phone"].ToString(),
                Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                Region = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "Region")? reader["Region"]?.ToString(): null,

                Branch = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "Branch")? reader["Branch"]?.ToString(): null,

                City = reader["City"] == DBNull.Value ? null : reader["City"].ToString(),
                Address = reader["Address"] == DBNull.Value ? null : reader["Address"].ToString(),
                MobileNo = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "MobileNo")? reader["MobileNo"]?.ToString(): null,

                SAPCode = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "SAPCode")? reader["SAPCode"]?.ToString(): null,

                BankName = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "BankName")? reader["BankName"]?.ToString(): null,

                AccountNo = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "AccountNo")? reader["AccountNo"]?.ToString(): null,

                IFSCCode = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "IFSCCode")? reader["IFSCCode"]?.ToString(): null,

                ASC_GST = reader["ASC_GST"] == DBNull.Value ? null : reader["ASC_GST"].ToString(),
                ASC_State = reader["ASC_State"] == DBNull.Value ? null : reader["ASC_State"].ToString(),

                ASC_StateCode = reader.GetSchemaTable().Rows.Cast<DataRow>().Any(r => r["ColumnName"].ToString() == "ASC_StateCode")? reader["ASC_StateCode"]?.ToString(): null,

                BranchWarehouse = reader["BranchWarehouse"] == DBNull.Value ? null : reader["BranchWarehouse"].ToString(),
                CorrectionWarehouse = reader["CorrectionWarehouse"] == DBNull.Value ? null : reader["CorrectionWarehouse"].ToString(),
                CorrectionWarehousepincode = reader["CorrectionWarehousepincode"] == DBNull.Value? null : reader["CorrectionWarehousepincode"].ToString(),

                Pincode = reader["Pincode"] == DBNull.Value? (long?)null: Convert.ToInt64(reader["Pincode"])
            };

        }
    }
}
