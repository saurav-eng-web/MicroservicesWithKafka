using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IModelVariantService
{
    Task<ModelVariant> CreateModelVariantAsync(ModelVariant variant);
    Task<bool> UpdateModelVariantAsync(ModelVariant variant);
    Task<bool> DeleteModelVariantAsync(decimal variantId);
    Task<ModelVariant> GetModelVariantByIdAsync(decimal variantId);
    Task<List<ModelVariant>> GetAllModelVariantsAsync();
}