using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IASMMasterService
    {
        Task<ASMMaster> CreateASMMasterAsync(ASMMaster asm);
        Task<bool> UpdateASMMasterAsync(ASMMaster asm);
        Task<bool> DeleteASMMasterAsync(int id);
        Task<ASMMaster?> GetASMMasterByIdAsync(int id);
        Task<List<ASMMaster>> GetAllASMMastersAsync();

        Task<List<ASMMaster>> GetAllRSMMastersAsync();
    }
}
