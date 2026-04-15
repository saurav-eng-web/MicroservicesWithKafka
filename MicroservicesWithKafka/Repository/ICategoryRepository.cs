using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ICategoryRepository
    {
        Task<CategoryMaster> CreateCategoryAsync(CategoryMaster category);
        Task<bool> UpdateCategoryAsync(CategoryMaster category);
        Task<CategoryMaster> GetCategoryByIdAsync(decimal catId);
        Task<List<CategoryMaster>> GetAllCategoriesAsync();

    }
}
