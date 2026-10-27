using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticCourierRepository : ILogisticCourierRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticCourierRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<IEnumerable<CourierViewModel>> GetCouriersAsync()
        {
            var couriers = new List<CourierViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                
                // Translated exactly from CourierAdd.aspx.vb Page_Init
                string query = "select CourierName, CourierName1 from CourierMaster";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            couriers.Add(new CourierViewModel
                            {
                                CourierCode = reader["CourierName"] != DBNull.Value ? reader["CourierName"].ToString() : string.Empty,
                                CourierName = reader["CourierName1"] != DBNull.Value ? reader["CourierName1"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }
            return couriers;
        }

        public async Task<bool> CourierExistsAsync(string courierCode)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                
                // Translated exactly from CourierAdd.aspx.vb btnSave_Click exists check
                string query = "select count(*) from CourierMaster where CourierName=@CourierCode";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@CourierCode", courierCode);
                    
                    var count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task<bool> AddCourierAsync(CourierViewModel courier)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                
                // Translated exactly from CourierAdd.aspx.vb btnSave_Click insert
                string query = "insert into CourierMaster (CourierName,CourierName1) values (@CourierCode, @CourierName)";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@CourierCode", courier.CourierCode);
                    command.Parameters.AddWithValue("@CourierName", courier.CourierName);
                    
                    int rows = await command.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        // Translated from CourierMapping.aspx.vb MappingDownload_Click
        public async Task<IEnumerable<CourierMappingViewModel>> GetMappedCoursAsync()
        {
            var result = new List<CourierMappingViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                string query = @"select (select CourierName from CourierMaster where CourierMaster.id=CourMapping.CourierID) CourierCode,
                    (select CourierName from CourierMaster where CourierMaster.id=CourMapping.CourierID) CourierName,
                    Pincode, Slab, [Priority], convert(nvarchar,Tdate,101) [Date]
                    from CourMapping";
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new CourierMappingViewModel
                            {
                                CourierCode = reader["CourierCode"] != DBNull.Value ? reader["CourierCode"].ToString() : string.Empty,
                                CourierName = reader["CourierName"] != DBNull.Value ? reader["CourierName"].ToString() : string.Empty,
                                Pincode     = reader["Pincode"]     != DBNull.Value ? reader["Pincode"].ToString()     : string.Empty,
                                Slab        = reader["Slab"]        != DBNull.Value ? reader["Slab"].ToString()        : string.Empty,
                                Priority    = reader["Priority"]    != DBNull.Value ? reader["Priority"].ToString()    : string.Empty,
                                Date        = reader["Date"]        != DBNull.Value ? reader["Date"].ToString()        : string.Empty
                            });
                        }
                    }
                }
            }
            return result;
        }

        // Translated from CourierMapping.aspx.vb MappingNotDownload_Click
        public async Task<IEnumerable<CourierMappingViewModel>> GetUnmappedCoursAsync()
        {
            var result = new List<CourierMappingViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                string query = @"select Courier as Couriercode, Pincode, Slab, [Priority], [Action],
                    convert(nvarchar,Tdate,101) [Date] from CourMappingUpload where isUpdate=0";
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new CourierMappingViewModel
                            {
                                CourierCode = reader["Couriercode"] != DBNull.Value ? reader["Couriercode"].ToString() : string.Empty,
                                Pincode     = reader["Pincode"]     != DBNull.Value ? reader["Pincode"].ToString()     : string.Empty,
                                Slab        = reader["Slab"]        != DBNull.Value ? reader["Slab"].ToString()        : string.Empty,
                                Priority    = reader["Priority"]    != DBNull.Value ? reader["Priority"].ToString()    : string.Empty,
                                Action      = reader["Action"]      != DBNull.Value ? reader["Action"].ToString()      : string.Empty,
                                Date        = reader["Date"]        != DBNull.Value ? reader["Date"].ToString()        : string.Empty
                            });
                        }
                    }
                }
            }
            return result;
        }

        // Translated from CourierMapping.aspx.vb bindgrid — SqlBulkCopy into CourMappingUpload then call Set_CourierMapping SP
        public async Task<bool> UploadCourierMappingAsync(IEnumerable<CourierMappingUploadViewModel> rows)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                var dt = new DataTable();
                dt.Columns.Add("Courier",  typeof(string));
                dt.Columns.Add("Pincode",  typeof(string));
                dt.Columns.Add("Slab",     typeof(string));
                dt.Columns.Add("Priority", typeof(string));
                dt.Columns.Add("Action",   typeof(string));

                foreach (var row in rows)
                    dt.Rows.Add(row.CourierCode, row.Pincode, row.Slab, row.Priority, row.Action);

                using (var bulkCopy = new Microsoft.Data.SqlClient.SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "CourMappingUpload";
                    bulkCopy.ColumnMappings.Add("Courier",  "Courier");
                    bulkCopy.ColumnMappings.Add("Pincode",  "Pincode");
                    bulkCopy.ColumnMappings.Add("Slab",     "Slab");
                    bulkCopy.ColumnMappings.Add("Priority", "Priority");
                    bulkCopy.ColumnMappings.Add("Action",   "Action");
                    await bulkCopy.WriteToServerAsync(dt);
                }

                // Call SP to process the uploaded mapping data
                using (var command = new SqlCommand("Set_CourierMapping", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await command.ExecuteNonQueryAsync();
                }
            }
            return true;
        }
    }
}
