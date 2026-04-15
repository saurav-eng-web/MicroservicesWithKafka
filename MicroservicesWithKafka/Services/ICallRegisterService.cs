using MicroservicesWithKafka.Models;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ICallRegisterService
    {
        //Task<CallRegisterModel> CreateCallAsync(CallRegisterModel model);
        //Task<List<CallRegisterModel>> GetAllCallsAsync();
        //Task<List<CallRegisterModel>> GetCallsByStatusAsync(string complaintStatus);
        //Task<List<CallRegisterModel>> GetTechnicalApprovalsAsync();

        //Task<CallRegisterModel?> GetByCallNumberAsync(string callNumber);
        //Task<bool> UpdateToAllotAsync(string callNumber, string enteredBy, int enteredById, string remarks, string ascCode);

        Task<string?> CreateComplaintAsync(CallRegisterModel model);
        Task<CallRegisterModel?> GetComplaintByTicketNoAsync(string ticketNo);
        Task<List<CallRegisterModel>> GetComplaintsByStatusAsync(string? complaintStatus, string? complaintSubStatus, int? role, int? bid);

        Task<bool> ApproveOrUpdateComplaintAsync(string ticketNo,int userId ,string action,string? remarks,
       string? ascCode, string? dropdownSelection, string? complaintStatus, string? enteredBy, string? category);

        // 🔹 New method for updating complaints
        Task<List<CallRegisterModel>> UpdateComplaintAsync(string ticketNo, CallRegisterModel updatedModel);
    }

}
