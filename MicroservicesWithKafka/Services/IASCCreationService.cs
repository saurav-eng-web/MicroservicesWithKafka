using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IASCCreationService
    {
        Task<ASCCreation> CreateASCCreationAsync(ASCCreation asc);
        Task<bool> UpdateASCCreationAsync(ASCCreation asc);
        Task<bool> DeleteASCCreationAsync(int id);
        Task<ASCCreation?> GetASCCreationByIdAsync(int id);
        Task<List<ASCCreation>> GetAllASCCreationsAsync();
    }
}
