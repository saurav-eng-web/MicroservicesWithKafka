using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public interface ILogisticStatusViewRepository
    {
        // Translated from StatusView.aspx.vb Page_Load — each type maps to a specific table query
        Task<IEnumerable<StatusViewViewModel>> GetStatusViewAsync(string type);
    }

    public class LogisticStatusViewRepository : ILogisticStatusViewRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticStatusViewRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        // Translated exactly from StatusView.aspx.vb Page_Load — 6 query branches by type
        public async Task<IEnumerable<StatusViewViewModel>> GetStatusViewAsync(string type)
        {
            var result = new List<StatusViewViewModel>();
            string query = string.Empty;

            // Each case maps exactly to the old Request.QueryString("type") branch in StatusView.aspx.vb
            switch (type)
            {
                case "DefectivePart":
                    query = "select DefPO_No As ChallanNo,TypeOfChallan,PickupBoy,PickupMobile,DocketStatus from Tbl_DefPOMaster WHERE DocketStatus='Dispatched'";
                    break;
                case "DefectiveDOA":
                    query = "select DefDOANWH_PONO As ChallanNo,TypeOfChallan,PickupBoy,PickupMobile,DocketStatus from tbl_DOADefectiveNWH WHERE DocketStatus='Dispatched'";
                    break;
                case "GoodPart":
                    query = "select Gud_No As ChallanNo,TypeOfChallan,PickupBoy,PickupMobile,DocketStatus from Tbl_GudPOMaster WHERE DocketStatus='Dispatched'";
                    break;
                case "GoodMachine":
                    query = "select POno As ChallanNo,TypeOfChallan,PickupBoy,PickupMobile,DocketStatus from tbl_SfFGStock WHERE DocketStatus='Dispatched' and MiscBin=0";
                    break;
                case "MismatchPart":
                    query = "select Mis_No As ChallanNo,TypeOfChallan,PickupBoy,PickupMobile,DocketStatus from Tbl_MisPOMaster WHERE DocketStatus='Dispatched'";
                    break;
                case "MismatchMachine":
                    query = "select POno As ChallanNo,TypeOfChallan,PickupBoy,PickupMobile,DocketStatus from tbl_SfFGStock WHERE DocketStatus='Dispatched' and MiscBin=1";
                    break;
                default:
                    return result;
            }

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new StatusViewViewModel
                            {
                                ChallanNo     = reader["ChallanNo"]     != DBNull.Value ? reader["ChallanNo"].ToString()     : string.Empty,
                                TypeOfChallan = reader["TypeOfChallan"] != DBNull.Value ? reader["TypeOfChallan"].ToString() : string.Empty,
                                PickupBoy     = reader["PickupBoy"]     != DBNull.Value ? reader["PickupBoy"].ToString()     : string.Empty,
                                PickupMobile  = reader["PickupMobile"]  != DBNull.Value ? reader["PickupMobile"].ToString()  : string.Empty,
                                DocketStatus  = reader["DocketStatus"]  != DBNull.Value ? reader["DocketStatus"].ToString()  : string.Empty
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}
