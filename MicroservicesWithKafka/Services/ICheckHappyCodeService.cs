using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ICheckHappyCodeService
    {
        Task<dynamic> CheckAsync(string TK, string UserName);
    }
}
