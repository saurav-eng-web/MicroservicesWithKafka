using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class ASCCreationService : IASCCreationService
    {
        private readonly IASCCreationRepository _ascCreationRepository;

        public ASCCreationService(IASCCreationRepository ascCreationRepository)
        {
            _ascCreationRepository = ascCreationRepository;
        }

        public async Task<ASCCreation> CreateASCCreationAsync(ASCCreation asc)
        {
            return await _ascCreationRepository.CreateASCCreationAsync(asc);
        }

        public async Task<bool> UpdateASCCreationAsync(ASCCreation asc)
        {
            return await _ascCreationRepository.UpdateASCCreationAsync(asc);
        }

        public async Task<bool> DeleteASCCreationAsync(int id)
        {
            return await _ascCreationRepository.DeleteASCCreationAsync(id);
        }

        public async Task<ASCCreation?> GetASCCreationByIdAsync(int id)
        {
            return await _ascCreationRepository.GetASCCreationByIdAsync(id);
        }

        public async Task<List<ASCCreation>> GetAllASCCreationsAsync()
        {
            return await _ascCreationRepository.GetAllASCCreationsAsync();
        }
    }
}
