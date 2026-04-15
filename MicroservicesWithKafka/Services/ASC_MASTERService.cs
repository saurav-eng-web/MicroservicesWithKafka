using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class ASC_MASTERService : IASC_MASTERService
    {
        private readonly IASC_MASTERRepository _ascRepository;

        public ASC_MASTERService(IASC_MASTERRepository ascRepository)
        {
            _ascRepository = ascRepository;
        }

        public async Task<ASC_MASTER> CreateASCAsync(ASC_MASTER asc)
        {
            return await _ascRepository.CreateAscAsync(asc);
        }

        public async Task<bool> UpdateASCAsync(ASC_MASTER asc)
        {
            return await _ascRepository.UpdateAscAsync(asc);
        }

        public async Task<bool> DeleteASCAsync(long ascIntCode)
        {
            return await _ascRepository.DeleteAscAsync(ascIntCode);
        }

        public async Task<ASC_MASTER?> GetASCByIdAsync(long ascIntCode)
        {
            return await _ascRepository.GetAscByIdAsync(ascIntCode);
        }

        public async Task<ASC_MASTER?> GetASCByasccodeAsync(string asccode)
        {
            return await _ascRepository.GetASCByasccodeAsync(asccode);
        }

        public async Task<List<ASC_MASTER>> GetAllASCsAsync()
        {
            return await _ascRepository.GetAllAscAsync();
        }

        public async Task<List<Dictionary<string, object>>> GetAscByRoleAsync(string role, int id)
        {
            return await _ascRepository.GetAscByRoleAsync(role, id);
        }
    }
}
