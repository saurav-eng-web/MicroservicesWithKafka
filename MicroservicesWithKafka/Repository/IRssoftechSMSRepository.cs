using MicroservicesWithKafka.Models;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IRssoftechSMSRepository
    {
        Task<bool> InsertSentSMSAsync(RssoftechSMS sms);
    }
}
