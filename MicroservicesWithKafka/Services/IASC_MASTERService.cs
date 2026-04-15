using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IASC_MASTERService
    {
        Task<ASC_MASTER> CreateASCAsync(ASC_MASTER asc);
        Task<bool> UpdateASCAsync(ASC_MASTER asc);
        Task<bool> DeleteASCAsync(long ascIntCode);
        Task<ASC_MASTER?> GetASCByIdAsync(long ascIntCode);
        Task<ASC_MASTER?> GetASCByasccodeAsync(string asccode);
        Task<List<ASC_MASTER>> GetAllASCsAsync();
        Task<List<Dictionary<string, object>>> GetAscByRoleAsync(string role, int id);
    }
}
