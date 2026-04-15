using MicroservicesWithKafka.Models;

public interface IModelVariantRepository
{
    Task<ModelVariant> CreateAsync(ModelVariant variant);
    Task<bool> UpdateAsync(ModelVariant variant);
    Task<bool> DeleteAsync(decimal variantId);
    Task<ModelVariant> GetByIdAsync(decimal variantId);
    Task<List<ModelVariant>> GetAllAsync();
}