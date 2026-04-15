using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class MstColorService : IMstColorService
    {
        private readonly IMstColorRepository _repository;

        public MstColorService(IMstColorRepository repository)
        {
            _repository = repository;
        }

        public async Task<TblMstColor> CreateColorAsync(TblMstColor color)
        {
            return await _repository.CreateColorAsync(color);
        }

        public async Task<bool> UpdateColorAsync(TblMstColor color)
        {
            return await _repository.UpdateColorAsync(color);
        }

        public async Task<bool> DeleteColorAsync(decimal colorId)
        {
            return await _repository.DeleteColorAsync(colorId);
        }

        public async Task<TblMstColor?> GetColorByIdAsync(decimal colorId)
        {
            return await _repository.GetColorByIdAsync(colorId);
        }

        public async Task<List<TblMstColor>> GetAllColorsAsync()
        {
            return await _repository.GetAllColorsAsync();
        }
    }
}
