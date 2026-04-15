using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IASMMasterRepository
    {
        Task<ASMMaster> CreateASMMasterAsync(ASMMaster asm);
        Task<ASMMaster?> GetASMMasterByIdAsync(int id);
        Task<List<ASMMaster>> GetAllASMMastersAsync();

        Task<List<ASMMaster>> GetAllRSMMastersAsync();
        Task<bool> UpdateASMMasterAsync(ASMMaster asm);
        Task<bool> DeleteASMMasterAsync(int id);
    }
}
