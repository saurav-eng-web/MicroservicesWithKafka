using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ICancellationService
    {
        Task<bool> HandleCancellationRequestAsync(
            string TICKET_NO,
            string remark,
            string type,
            string action,
            string remarkS,
            int uid);
    }
}
