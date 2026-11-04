using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticDashboardRepository : ILogisticDashboardRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticDashboardRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        // Translated from Dashboard.aspx.vb Page_Load – calls sp_LogisticDashBoard then reads courier availability
        public async Task<DashboardSummaryViewModel> GetDashboardSummaryAsync()
        {
            var summary = new DashboardSummaryViewModel();

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                // Step 1: Call SP sp_LogisticDashBoard to get challan counts
                using (var command = new SqlCommand("sp_LogisticDashBoard", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Column names taken exactly from Dashboard.aspx.vb ds2.Tables(0).Rows(0).Item(...)
                            summary.DefectivePart   = reader["Tbl_DefPOMaster"]      != DBNull.Value ? reader["Tbl_DefPOMaster"].ToString()      : "0";
                            summary.DefectiveDOA    = reader["tbl_DOADefectiveNWH"]  != DBNull.Value ? reader["tbl_DOADefectiveNWH"].ToString()  : "0";
                            summary.GoodPart        = reader["Tbl_GudPOMaster"]      != DBNull.Value ? reader["Tbl_GudPOMaster"].ToString()      : "0";
                            summary.GoodMachine     = reader["tbl_SfFGStock"]        != DBNull.Value ? reader["tbl_SfFGStock"].ToString()        : "0";
                            summary.MismatchPart    = reader["Tbl_MisPOMaster"]      != DBNull.Value ? reader["Tbl_MisPOMaster"].ToString()      : "0";
                            summary.MismatchMachine = reader["tbl_SfFGStock"]        != DBNull.Value ? reader["tbl_SfFGStock"].ToString()        : "0";
                        }
                    }
                }

                // Step 2: Get courier availability counts
                string courierQuery = @"select (select CourierName from CourierMaster where CourierMaster.id=DocMaster.Courierid) CourierName,
                    count(*) Avi from DocMaster where IsUsed=0 group by Courierid order by Avi desc";

                using (var command = new SqlCommand(courierQuery, connection))
                {
                    command.CommandType = CommandType.Text;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            summary.CourierAvailability.Add(new CourierAvailabilityViewModel
                            {
                                CourierName = reader["CourierName"] != DBNull.Value ? reader["CourierName"].ToString() : string.Empty,
                                Avi         = reader["Avi"] != DBNull.Value         ? Convert.ToInt32(reader["Avi"])   : 0
                            });
                        }
                    }
                }
            }

            return summary;
        }
    }
}
