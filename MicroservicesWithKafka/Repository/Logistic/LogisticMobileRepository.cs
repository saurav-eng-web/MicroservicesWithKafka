using Dapper;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticMobileRepository : ILogisticMobileRepository
    {
        private const int ReportTimeoutSeconds = 600;
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticMobileRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private static List<IDictionary<string, object>> ToRows(IEnumerable<dynamic> rows)
            => rows.Select(r => (IDictionary<string, object>)r).ToList();

        public async Task<IEnumerable<dynamic>> GetAscListAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            return await connection.QueryAsync("ASClistm", new { id = "", Role = "LOG" },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> GetChallanListAsync(string sfId, string spType, int month, int year)
        {
            using var connection = _sqlConnectionService.GetConnection();
            return await connection.QueryAsync("PrintChallanListm",
                new { SfId = sfId, Type = spType, Month = month.ToString(), Year = year.ToString() },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<MobileChallanPrintViewModel> GetChallanPrintAsync(string poNo, string spType)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var lines = await connection.QueryAsync("ChallanLinem", new { PoNo = poNo, Type = spType },
                commandType: CommandType.StoredProcedure);
            var header = await connection.QueryAsync("invoicedetail1m", new { PONum = poNo, Type = spType },
                commandType: CommandType.StoredProcedure);

            return new MobileChallanPrintViewModel
            {
                Header = ToRows(header).FirstOrDefault(),
                Lines = ToRows(lines)
            };
        }

        public async Task<IEnumerable<dynamic>> GetReportAsync(string type, string fromDate, string toDate, string userId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            return await connection.QueryAsync("reportsm",
                new { fromdate = fromDate, todate = toDate, type, userid = userId },
                commandType: CommandType.StoredProcedure, commandTimeout: ReportTimeoutSeconds);
        }

        public async Task<IEnumerable<dynamic>> SearchComplaintsAsync(string text, string userId, string ipAddress)
        {
            using var connection = _sqlConnectionService.GetConnection();
            return await connection.QueryAsync("Search_Customer25",
                new { Text = text, UserId = userId, ipaddress = ipAddress },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<MobileComplaintHistoryViewModel> GetComplaintHistoryAsync(string ticketKey, string userId)
        {
            var sets = new List<List<IDictionary<string, object>>>();
            using var connection = _sqlConnectionService.GetConnection();

            using (var multi = await connection.QueryMultipleAsync("sp_ReminderHistory6",
                       new { TicketKey = ticketKey, Userid = userId }, commandType: CommandType.StoredProcedure))
            {
                while (!multi.IsConsumed)
                    sets.Add(ToRows(await multi.ReadAsync()));
            }

            var documents = await connection.QueryAsync("sp_GetSupportingDocumentmah",
                new { Ticket_Key = ticketKey, Userid = userId }, commandType: CommandType.StoredProcedure);

            List<IDictionary<string, object>> Set(int index)
                => index < sets.Count ? sets[index] : new List<IDictionary<string, object>>();

            return new MobileComplaintHistoryViewModel
            {
                Customer        = Set(0),
                CustomerAddress = Set(1),
                Product         = Set(2),
                Complaint       = Set(3),
                ComplaintDetail = Set(4),
                History         = Set(9),
                Documents       = ToRows(documents)
            };
        }
    }
}
