using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IModelRepository
    {
        Task<ModelMasterViewModel> CreateModelAsync(ModelMasterViewModel model);
        Task<bool> UpdateModelAsync(ModelMasterViewModel model);
        Task<bool> DeleteModelAsync(decimal modelId);
        Task<ModelMasterViewModel> GetModelByIdAsync(decimal modelId);
        Task<List<ModelMasterViewModel>> GetAllModelsAsync();

        Task<List<ModelMasterViewModel>> GetModelsByProductIdAsync(decimal productId);
    }
}
