using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IBrandMasterRepository
    {
        Task<BrandMasterViewModel> CreateBrandAsync(BrandMasterViewModel brand);
        Task<bool> UpdateBrandAsync(BrandMasterViewModel brand);
        Task<bool> DeleteBrandAsync(decimal brandId);
        Task<BrandMasterViewModel?> GetBrandByIdAsync(decimal brandId);
        Task<List<BrandMasterViewModel>> GetAllBrandsAsync();
    }
}
