using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class LogisticCnService : ILogisticCnService
    {
        private const int MaxUploadRows = 50000;
        private readonly ILogisticCnRepository _repository;

        public LogisticCnService(ILogisticCnRepository repository)
        {
            _repository = repository;
        }

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string userId)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "upload", "search");

            if (type == "search")
                return (await SearchAsync(LogisticPayload.Str(json, "ticketNo")), 200, "Record found Successfully");

            var rows = LogisticPayload.DataArray(json).ToObject<List<CnLogisticUploadViewModel>>() ?? new List<CnLogisticUploadViewModel>();
            var result = await UploadAsync(rows, userId);
            return (null, result.Success ? 200 : 400, result.Message);
        }

        private async Task<(bool Success, string Message)> UploadAsync(IEnumerable<CnLogisticUploadViewModel> rows, string userId)
        {
            if (rows == null)
                return (false, "No data provided.");
            if (string.IsNullOrWhiteSpace(userId) || userId == "0")
                return (false, "Logged-in user could not be identified.");

            var list = rows.Where(r => r != null && !(string.IsNullOrWhiteSpace(r.CNno) && string.IsNullOrWhiteSpace(r.DispatchWarehouse)
                                                      && string.IsNullOrWhiteSpace(r.DocketNo) && string.IsNullOrWhiteSpace(r.DispatchStatus)))
                           .ToList();
            if (list.Count == 0)
                return (false, "No data provided.");
            if (list.Count > MaxUploadRows)
                return (false, $"Too many rows. Maximum {MaxUploadRows} rows per upload.");

            var missing = list.Select((r, i) => new { r, Row = i + 1 })
                              .Where(x => string.IsNullOrWhiteSpace(x.r.CNno))
                              .Select(x => x.Row).Take(20).ToList();
            if (missing.Count > 0)
                return (false, $"CNno is required. Check row(s): {string.Join(", ", missing)}.");

            int count = await _repository.UploadAsync(list, userId);
            return (true, $"DATA IMPORTED SUCCESSFULLY. {count} row(s) uploaded.");
        }

        private async Task<IEnumerable<dynamic>> SearchAsync(string? ticketNo)
        {
            if (string.IsNullOrWhiteSpace(ticketNo))
                return await _repository.GetUploadedCnAsync();
            return await _repository.GetChallanByTicketAsync(ticketNo.Trim());
        }
    }
}
