using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ISMSService
    {
        Task<bool> SendSMSAsync(string message, string mobile, string ticketKey, string source);
    }
}
