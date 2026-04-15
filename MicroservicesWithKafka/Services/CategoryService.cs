using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryMaster> CreateCategoryAsync(CategoryMaster category)
        {
            return await _categoryRepository.CreateCategoryAsync(category);
        }

        public async Task<bool> UpdateCategoryAsync(CategoryMaster category)
        {
            return await _categoryRepository.UpdateCategoryAsync(category);
        }

        public async Task<CategoryMaster> GetCategoryByIdAsync(decimal catId)
        {
            return await _categoryRepository.GetCategoryByIdAsync(catId);
        }
        public async Task<List<CategoryMaster>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }

    }
}
