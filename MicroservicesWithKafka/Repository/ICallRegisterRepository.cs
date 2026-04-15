using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface ICallRegisterRepository
    {
        /// <summary>
        /// Creates a new complaint/call and returns the generated ReturnNo.
        /// </summary>
        Task<string?> CreateComplaintAsync(CallRegisterModel model);
        Task<CallRegisterModel?> GetComplaintByTicketNoAsync(string ticketNo);

        Task<List<CallRegisterModel>> GetComplaintsByStatusAsync(string? complaintStatus, string? complaintSubStatus,int? role,int? bid);

        Task<bool> ApproveOrUpdateComplaintAsync(string ticketNo,int userId,string action,string? remarks,
       string? ascCode,string? dropdownSelection,string? complaintStatus,string? enteredBy,string? category);

        // 🔹 New method for updating complaint details
        Task<List<CallRegisterModel>> UpdateComplaintAsync(string ticketNo, CallRegisterModel updatedModel);
    }
}
