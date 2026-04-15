using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class ProductMasterService : IProductMasterService
    {
        private readonly IProductMasterRepository _productRepo;

        public ProductMasterService(IProductMasterRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<ProductMasterModel>> GetAllAsync()
        {
            return await _productRepo.GetAll();
        }

        public async Task<IEnumerable<ProductMasterModel>> BrandCategoryId(decimal Brandid, decimal catid)
        {
            return await _productRepo.BrandCategoryId(Brandid, catid);
        }

        public async Task<ProductMasterModel?> GetByIdAsync(decimal id)
        {
            return await _productRepo.GetById(id);
        }

        public async Task AddAsync(ProductMasterModel model)
        {
            await _productRepo.Add(model);
        }

        public async Task AddMultipleAsync(IEnumerable<ProductMasterModel> models)
        {
            await _productRepo.AddMultiple(models);
        }

        public async Task DeleteAsync(decimal id)
        {
            await _productRepo.Delete(id);
        }
    }
}
