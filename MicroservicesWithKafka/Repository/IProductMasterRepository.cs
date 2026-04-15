using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IProductMasterRepository
    {
        Task<IEnumerable<ProductMasterModel>> GetAll();
        Task<IEnumerable<ProductMasterModel>> BrandCategoryId(decimal Brandid, decimal catid);
        Task<ProductMasterModel?> GetById(decimal id);
        Task Add(ProductMasterModel model);
        Task AddMultiple(IEnumerable<ProductMasterModel> models);
        Task Delete(decimal id);
    }
}
