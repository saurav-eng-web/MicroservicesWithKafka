using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repositorie;
using MicroservicesWithKafka.Services;

public class PODetailService : IPODetailService
{
    private readonly IPODetailRepository _repo;
    private readonly IComplaintHistoryService _complaintHistoryService;

    public PODetailService(IPODetailRepository repo,IComplaintHistoryService complaintHistoryService)
    {
        _repo = repo;
        _complaintHistoryService = complaintHistoryService;
    }

    public async Task<PODetailResponse> ManagePODetailAsync(PODetailRequest request)
    {
        var result = await _repo.ManagePODetailAsync(request);

        // Convert BOOL to INT manually
        foreach (var row in result)
        {
            row.ProcessToBranch = BoolToInt(row.ProcessToBranch);
            row.Dispatch = BoolToInt(row.Dispatch);
            row.Backorder = BoolToInt(row.Backorder);
            row.SendNWH = BoolToInt(row.SendNWH);
            row.SendVendor = BoolToInt(row.SendVendor);
            row.ValueApproval = BoolToInt(row.ValueApproval);
            row.ispa = BoolToInt(row.ValueApproval);
            row.IsPackingSlipUsed = BoolToInt(row.IsPackingSlipUsed);
        }

        var first = result.FirstOrDefault();

        // Run ONLY when StatusCode == 200 AND Action == UPDATE
        if (first?.StatusCode == 200 && request.Action?.Equals("UPDATE", StringComparison.OrdinalIgnoreCase) == true)
        {
            var complaintHistory = new ComplaintHistory
            {
                TicketNo = first.Ticket_no,
                ComplaintStatus = first.Complaint_Status,   //FIXED
                ComplaintSubStatus =$"{first.Complaint_Substatus} {first.dlcity}", // FIXED
                EnteredBy = first.EnterBy,
                EnteredByID = null,
                Tdate = DateTime.UtcNow
            };


            await _complaintHistoryService.AddComplaintAsync(complaintHistory);
        }

        return new PODetailResponse
        {
            StatusCode = first?.StatusCode ?? 500,
            Message = first?.Message ?? "Unknown error",
            Data = result
        };
    }

    private int BoolToInt(object value)
    {
        if (value == null) return 0;
        return value is bool b ? (b ? 1 : 0) : Convert.ToInt32(value);
    }
}