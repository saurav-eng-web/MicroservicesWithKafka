using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class BrandMasterService : IBrandMasterService
    {
        private readonly IBrandMasterRepository _brandMasterRepository;

        public BrandMasterService(IBrandMasterRepository brandMasterRepository)
        {
            _brandMasterRepository = brandMasterRepository;
        }

        public async Task<BrandMasterViewModel> CreateBrandAsync(BrandMasterViewModel brand)
        {
            return await _brandMasterRepository.CreateBrandAsync(brand);
        }

        public async Task<bool> UpdateBrandAsync(BrandMasterViewModel brand)
        {
            return await _brandMasterRepository.UpdateBrandAsync(brand);
        }

        public async Task<bool> DeleteBrandAsync(decimal brandId)
        {
            return await _brandMasterRepository.DeleteBrandAsync(brandId);
        }

        public async Task<BrandMasterViewModel?> GetBrandByIdAsync(decimal brandId)
        {
            return await _brandMasterRepository.GetBrandByIdAsync(brandId);
        }

        public async Task<List<BrandMasterViewModel>> GetAllBrandsAsync()
        {
            return await _brandMasterRepository.GetAllBrandsAsync();
        }
    }
}
