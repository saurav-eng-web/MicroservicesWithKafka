using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class ModelService : IModelService
    {
        private readonly IModelRepository _modelRepository;

        public ModelService(IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;
        }

        public async Task<ModelMasterViewModel> CreateModelAsync(ModelMasterViewModel model)
        {
            // Optional: Add validation/business logic here
            return await _modelRepository.CreateModelAsync(model);
        }

        public async Task<bool> UpdateModelAsync(ModelMasterViewModel model)
        {
            // Optional: Add validation/business logic here
            return await _modelRepository.UpdateModelAsync(model);
        }

        public async Task<bool> DeleteModelAsync(decimal modelId)
        {
            return await _modelRepository.DeleteModelAsync(modelId);
        }

        public async Task<ModelMasterViewModel> GetModelByIdAsync(decimal modelId)
        {
            return await _modelRepository.GetModelByIdAsync(modelId);
        }

        public async Task<List<ModelMasterViewModel>> GetModelsByProductIdAsync(decimal productId)
        {
            return await _modelRepository.GetModelsByProductIdAsync(productId);
        }

        public async Task<List<ModelMasterViewModel>> GetAllModelsAsync()
        {
            return await _modelRepository.GetAllModelsAsync();
        }
    }
}
