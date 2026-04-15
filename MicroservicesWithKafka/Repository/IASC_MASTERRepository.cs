

using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IASC_MASTERRepository
    {
        Task<ASC_MASTER> CreateAscAsync(ASC_MASTER asc);
        Task<bool> UpdateAscAsync(ASC_MASTER asc);
        Task<bool> DeleteAscAsync(long ascIntCode);
        Task<ASC_MASTER?> GetAscByIdAsync(long ascIntCode);
        Task<ASC_MASTER?> GetASCByasccodeAsync(string asccode);
        Task<List<ASC_MASTER>> GetAllAscAsync();
        Task<List<Dictionary<string, object>>> GetAscByRoleAsync(string role, int id);
    }
}
