using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class ChallanService : IChallanService
    {
        private readonly IChallanRepository _repo;
        private readonly IComplaintHistoryService _complaintHistoryService;

        public ChallanService(IChallanRepository repo, IComplaintHistoryService complaintHistoryService)
        {
            _repo = repo;
            _complaintHistoryService = complaintHistoryService;
        }

        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
           ProcessAsync(ProcessChallanRequest request)
        {
            var (data, code, msg) =
                await _repo.ProcessChallanAsync(request);

            // CALL HISTORY ONLY FOR Misc. FG Unit
            if (code == 200 &&  data.Any() && ShouldInsertHistory(request.Type))
            {
                await InsertComplaintHistoryAsync(data, request, msg);
            }

            return (data, code, msg);
        }

        private bool ShouldInsertHistory(string type)
        {
            var allowedTypes = new[]
            {
                "Fresh Faulty Spare",
                "Defective Spare",
                "DOA Unit",
                "Good Unit",
                "Misc. FG Unit"
            };

            return allowedTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
        }

        private async Task InsertComplaintHistoryAsync(
            IEnumerable<dynamic> data,
            ProcessChallanRequest request,
            string statusMsg)
        {
            foreach (var item in data)
            {
                IDictionary<string, object>? row =
                    item as IDictionary<string, object>;

                var complaintHistory = new ComplaintHistory
                {
                    TicketNo = row?["Ticket_no"]?.ToString(),
                    ComplaintStatus = row?["Complaint_status"]?.ToString() ?? "Open",
                    ComplaintSubStatus = row != null ? $"{row["Complaint_substatus"]} {row["dlcity"]}": statusMsg,
                    EnteredBy = request.Username,
                    EnteredByID = null,
                    Tdate = DateTime.UtcNow
                };

                await _complaintHistoryService.AddComplaintAsync(complaintHistory);
            }
        }


        //public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
        //    ProcessAsync(ProcessChallanRequest request)
        //{
        //    return await _repo.ProcessChallanAsync(request);
        //}
    }
}
