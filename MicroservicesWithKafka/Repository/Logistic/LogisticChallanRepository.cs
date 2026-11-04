using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticChallanRepository : ILogisticChallanRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticChallanRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private static HashSet<string> GetColumnNames(SqlDataReader reader)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
                names.Add(reader.GetName(i));
            return names;
        }

        private static string ReadString(SqlDataReader reader, HashSet<string> columns, string name)
        {
            if (!columns.Contains(name)) return string.Empty;
            var value = reader[name];
            return value == DBNull.Value ? string.Empty : value.ToString() ?? string.Empty;
        }

        private static int ReadInt(SqlDataReader reader, HashSet<string> columns, string name)
        {
            var text = ReadString(reader, columns, name);
            return int.TryParse(text, out var value) ? value : 0;
        }

        // ------------------------------------------------------------------
        // Logistic/PrintDefectiveChallan.aspx.vb (menu "Defective Challan")
        // PrintChallanList(@SfId=ascintcode, @Type, @Month, @Year).
        // Legacy grid: Challan No (ponumber, link), SF Name (sfname), Challan Date (date), Doc. No. (status).
        // spType is already mapped to the SP value (Defective, DOA, goodspare, GoodFG, MisFG, MisSpare).
        // ------------------------------------------------------------------
        public async Task<IEnumerable<DefectiveChallanViewModel>> GetPrintChallanListAsync(string sfId, string spType, int month, int year)
        {
            var result = new List<DefectiveChallanViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("PrintChallanList", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SfId", sfId);
                    command.Parameters.AddWithValue("@Type", spType);
                    // Legacy passed ddlmonth.SelectedValue / ddlyear.SelectedItem.Text (strings).
                    command.Parameters.AddWithValue("@Month", month.ToString());
                    command.Parameters.AddWithValue("@Year", year.ToString());

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        while (await reader.ReadAsync())
                        {
                            result.Add(new DefectiveChallanViewModel
                            {
                                Id       = ReadInt(reader, cols, "id"),
                                PoNumber = ReadString(reader, cols, "ponumber"),
                                SfName   = ReadString(reader, cols, "sfname"),
                                Date     = ReadString(reader, cols, "date"),
                                Status   = ReadString(reader, cols, "status")
                            });
                        }
                    }
                }
            }
            return result;
        }

        // ------------------------------------------------------------------
        // Logistic/PrintDOADefectiveChallan.aspx.vb (menu "Defective DOA Challan")
        // ------------------------------------------------------------------
        public async Task<IEnumerable<DefectiveChallanViewModel>> GetDOADefectiveChallansAsync(ChallanSearchRequestViewModel request)
        {
            var result = new List<DefectiveChallanViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                const string query = @"select po.DefDOANWH_ID as id,po.DefDOANWH_PONO as ponumber,sf.asc_name as sfname,
                    convert(nvarchar,po.DefDOANWH_DocateDate,101) as date,po.DefDOANWH_DocatNo as [status]
                    from dbo.tbl_DOADefectiveNWH po,dbo.ASC_MASTER sf
                    where po.DefDOANWH_SFID=sf.ascintcode
                    and datepart(mm,po.DefDOANWH_DocateDate)=@Month and datepart(yyyy,po.DefDOANWH_DocateDate)=@Year
                    and sf.asc_name=@AscName order by po.DefDOANWH_DocateDate desc";

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Month",   request.Month);
                    command.Parameters.AddWithValue("@Year",    request.Year);
                    command.Parameters.AddWithValue("@AscName", (object?)request.AscName ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        while (await reader.ReadAsync())
                        {
                            result.Add(new DefectiveChallanViewModel
                            {
                                Id       = ReadInt(reader, cols, "id"),
                                PoNumber = ReadString(reader, cols, "ponumber"),
                                SfName   = ReadString(reader, cols, "sfname"),
                                Date     = ReadString(reader, cols, "date"),
                                Status   = ReadString(reader, cols, "status")
                            });
                        }
                    }
                }
            }
            return result;
        }

        // ------------------------------------------------------------------
        // Logistic/PrintGoodChallan.aspx.vb — exact legacy query (Tbl_GudPOMaster uses Gud_* columns).
        // ------------------------------------------------------------------
        public async Task<IEnumerable<GoodPartChallanViewModel>> GetGoodChallansAsync(ChallanSearchRequestViewModel request)
        {
            var result = new List<GoodPartChallanViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                const string query = @"select Gud_No, convert(nvarchar,Gud_Date,101) as GudDate, Gud_DocNo, Gud_Remark
                    from Tbl_GudPOMaster as a
                    left join ASC_MASTER as b on a.Gud_SFId=b.ASCINTCODE
                    where b.ASC_NAME=@AscName
                    and DATEPART(mm,Gud_Date)=@Month and DATEPART(YY,Gud_Date)=@Year";

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@AscName", (object?)request.AscName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Month",   request.Month);
                    command.Parameters.AddWithValue("@Year",    request.Year);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        while (await reader.ReadAsync())
                        {
                            result.Add(new GoodPartChallanViewModel
                            {
                                GudNo     = ReadString(reader, cols, "Gud_No"),
                                GudDate   = ReadString(reader, cols, "GudDate"),
                                GudDocNo  = ReadString(reader, cols, "Gud_DocNo"),
                                GudRemark = ReadString(reader, cols, "Gud_Remark")
                            });
                        }
                    }
                }
            }
            return result;
        }

        // ------------------------------------------------------------------
        // Logistic/PrintGoodChallanMachine.aspx.vb
        // ------------------------------------------------------------------
        public async Task<IEnumerable<GoodChallanViewModel>> GetGoodMachineChallansAsync(ChallanSearchRequestViewModel request)
        {
            var result = new List<GoodChallanViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                const string query = @"SELECT FGS.ID,FGS.POno,
                    (SELECT TOP(1) Product FROM Product_Master WHERE ProductID IN (SELECT TOP(1) Product_ID FROM COMPLAINT WHERE TICKET_KEY=FGS.Ticket_key)) AS product,
                    (SELECT TOP(1) ModelNo FROM Model_Master WHERE ModelID=FGS.ModelId) AS Model,
                    FGS.NEwSerialNumber AS SLNO,
                    (SELECT TOP(1) TICKET_NO FROM COMPLAINT AS COMPLAINT_1 WHERE TICKET_KEY=FGS.Ticket_key) AS TN
                    FROM tbl_SfFGStock AS FGS
                    INNER JOIN COMPLAINT AS COMPLAINT_2 ON FGS.Ticket_key=COMPLAINT_2.TICKET_KEY
                    INNER JOIN ASC_MASTER as ac ON COMPLAINT_2.Sf_ID=ac.ASCINTCODE
                    WHERE FGS.isSend=1 and FGS.Ok=1 and ac.asc_name=@AscName
                    and FGS.PoStatus='Send To Nwh'
                    and DATEPART(mm,docdate)=@Month and DATEPART(YY,docdate)=@Year";

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@AscName", (object?)request.AscName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Month",   request.Month);
                    command.Parameters.AddWithValue("@Year",    request.Year);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        while (await reader.ReadAsync())
                        {
                            result.Add(new GoodChallanViewModel
                            {
                                Id       = ReadInt(reader, cols, "ID"),
                                PONo     = ReadString(reader, cols, "POno"),
                                Product  = ReadString(reader, cols, "product"),
                                Model    = ReadString(reader, cols, "Model"),
                                SerialNo = ReadString(reader, cols, "SLNO"),
                                TicketNo = ReadString(reader, cols, "TN")
                            });
                        }
                    }
                }
            }
            return result;
        }

        // ------------------------------------------------------------------
        // Logistic/PrintMisChallan.aspx.vb
        // ------------------------------------------------------------------
        public async Task<IEnumerable<MisChallanViewModel>> GetMisChallansAsync(ChallanSearchRequestViewModel request)
        {
            var result = new List<MisChallanViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                const string query = @"select Mis_No,convert(nvarchar,Mis_Date,101) as MisDate,Mis_DocNo,Mis_Remark
                    from Tbl_MisPOMaster as a
                    left join ASC_MASTER as b on a.Mis_SFId=b.ASCINTCODE
                    where b.asc_name=@AscName
                    and DATEPART(mm,Mis_Date)=@Month and DATEPART(YY,Mis_Date)=@Year";

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@AscName", (object?)request.AscName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Month",   request.Month);
                    command.Parameters.AddWithValue("@Year",    request.Year);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        while (await reader.ReadAsync())
                        {
                            result.Add(new MisChallanViewModel
                            {
                                MisNo     = ReadString(reader, cols, "Mis_No"),
                                MisDate   = ReadString(reader, cols, "MisDate"),
                                MisDocNo  = ReadString(reader, cols, "Mis_DocNo"),
                                MisRemark = ReadString(reader, cols, "Mis_Remark")
                            });
                        }
                    }
                }
            }
            return result;
        }

        // ------------------------------------------------------------------
        // Logistic/PrintMisChallanFG.aspx.vb — SF = Session("sidfic") in legacy (resolved by the controller).
        // ------------------------------------------------------------------
        public async Task<IEnumerable<GoodChallanViewModel>> GetMisChallanFGAsync(ChallanSearchRequestViewModel request)
        {
            var result = new List<GoodChallanViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                const string query = @"select POno,ID,
                    (Select Product from Product_Master where Product_Master.ProductID=tbl_SfFGStock.PRoductID) Product,
                    (Select ModelNo from Model_Master where Model_Master.ModelID=tbl_SfFGStock.ModelId) Model,
                    ((Select TICKET_NO from COMPLAINT where COMPLAINT.TICKET_KEY=tbl_SfFGStock.Ticket_key)+' - '+(select PackingSlip from Tbl_PoMaster_FG where MachineSlNo=tbl_SfFGStock.NEwSerialNumber)) TicketNo,
                    NEwSerialNumber As SerialNO
                    from tbl_SfFGStock
                    where isSend=1 and MiscBin=1
                    and @SfId in (Select Sf_ID from COMPLAINT where COMPLAINT.TICKET_KEY=tbl_SfFGStock.Ticket_key)
                    and DATEPART(mm,DocDate)=@Month and DATEPART(YY,DocDate)=@Year";

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@SfId",  (object?)request.SfId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Month", request.Month);
                    command.Parameters.AddWithValue("@Year",  request.Year);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        while (await reader.ReadAsync())
                        {
                            result.Add(new GoodChallanViewModel
                            {
                                Id       = ReadInt(reader, cols, "ID"),
                                PONo     = ReadString(reader, cols, "POno"),
                                Product  = ReadString(reader, cols, "Product"),
                                Model    = ReadString(reader, cols, "Model"),
                                SerialNo = ReadString(reader, cols, "SerialNO"),
                                TicketNo = ReadString(reader, cols, "TicketNo")
                            });
                        }
                    }
                }
            }
            return result;
        }

        // ------------------------------------------------------------------
        // SF dropdowns
        // ------------------------------------------------------------------

        // Legacy PrintDefectiveChallan.aspx.vb: ASClist(@id='', @Role='LOG') -> DataTextField asc_name, DataValueField ascintcode.
        public async Task<IEnumerable<AscOptionViewModel>> GetLogisticAscListAsync()
        {
            var result = new List<AscOptionViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("ASClist", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", "");
                    command.Parameters.AddWithValue("@Role", "LOG");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        while (await reader.ReadAsync())
                        {
                            result.Add(new AscOptionViewModel
                            {
                                AscIntCode = ReadString(reader, cols, "ascintcode"),
                                AscName    = ReadString(reader, cols, "asc_name")
                            });
                        }
                    }
                }
            }
            return result;
        }

        // Legacy DOA / Good / Good Machine / Mis pages:
        // select ASCINTCODE,ASC_NAME from ASC_MASTER order by ASC_Name
        public async Task<IEnumerable<AscOptionViewModel>> GetAscMasterListAsync()
        {
            var result = new List<AscOptionViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                const string query = "select ASCINTCODE, ASC_NAME from ASC_MASTER order by ASC_Name";

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        while (await reader.ReadAsync())
                        {
                            result.Add(new AscOptionViewModel
                            {
                                AscIntCode = ReadString(reader, cols, "ASCINTCODE"),
                                AscName    = ReadString(reader, cols, "ASC_NAME")
                            });
                        }
                    }
                }
            }
            return result;
        }
    }
}
