using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class ASMMasterService : IASMMasterService
    {
        private readonly IASMMasterRepository _asmRepository;

        public ASMMasterService(IASMMasterRepository asmRepository)
        {
            _asmRepository = asmRepository;
        }

        public async Task<ASMMaster> CreateASMMasterAsync(ASMMaster asm)
        {
            return await _asmRepository.CreateASMMasterAsync(asm);
        }

        public async Task<bool> UpdateASMMasterAsync(ASMMaster asm)
        {
            return await _asmRepository.UpdateASMMasterAsync(asm);
        }

        public async Task<bool> DeleteASMMasterAsync(int id)
        {
            return await _asmRepository.DeleteASMMasterAsync(id);
        }

        public async Task<ASMMaster?> GetASMMasterByIdAsync(int id)
        {
            return await _asmRepository.GetASMMasterByIdAsync(id);
        }

        public async Task<List<ASMMaster>> GetAllASMMastersAsync()
        {
            return await _asmRepository.GetAllASMMastersAsync();
        }

        public async Task<List<ASMMaster>> GetAllRSMMastersAsync()
        {
            return await _asmRepository.GetAllRSMMastersAsync();
        }
    }
}
