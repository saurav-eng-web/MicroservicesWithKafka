using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ICheckHappyCodeRepository
    {
        Task<dynamic> ExecuteAsync(string TK, string UserName);
    }
}
