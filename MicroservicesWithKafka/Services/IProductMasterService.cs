using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IProductMasterService
    {
        Task<IEnumerable<ProductMasterModel>> GetAllAsync();

        Task<IEnumerable<ProductMasterModel>> BrandCategoryId(decimal Brandid, decimal catid);
        Task<ProductMasterModel?> GetByIdAsync(decimal id);
        Task AddAsync(ProductMasterModel model);
        Task AddMultipleAsync(IEnumerable<ProductMasterModel> models);
        Task DeleteAsync(decimal id);
    }
}
