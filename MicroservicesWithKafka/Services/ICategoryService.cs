using MicroservicesWithKafka.Models;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ICategoryService
    {
        Task<CategoryMaster> CreateCategoryAsync(CategoryMaster category);
        Task<bool> UpdateCategoryAsync(CategoryMaster category);
        Task<CategoryMaster> GetCategoryByIdAsync(decimal catId);
        Task<List<CategoryMaster>> GetAllCategoriesAsync();

    }
}
