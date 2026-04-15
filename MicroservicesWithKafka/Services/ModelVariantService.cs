using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class ModelVariantService : IModelVariantService
    {
        private readonly IModelVariantRepository _modelVariantRepository;

        public ModelVariantService(IModelVariantRepository modelVariantRepository)
        {
            _modelVariantRepository = modelVariantRepository;
        }

        public async Task<ModelVariant> CreateModelVariantAsync(ModelVariant variant)
        {
            return await _modelVariantRepository.CreateAsync(variant);
        }

        public async Task<bool> UpdateModelVariantAsync(ModelVariant variant)
        {
            return await _modelVariantRepository.UpdateAsync(variant);
        }

        public async Task<bool> DeleteModelVariantAsync(decimal variantId)
        {
            return await _modelVariantRepository.DeleteAsync(variantId);
        }

        public async Task<ModelVariant> GetModelVariantByIdAsync(decimal variantId)
        {
            return await _modelVariantRepository.GetByIdAsync(variantId);
        }

        public async Task<List<ModelVariant>> GetAllModelVariantsAsync()
        {
            return await _modelVariantRepository.GetAllAsync();
        }
    }
}
