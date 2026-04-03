using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Repository;
using KASHOP.DAL.DTO.Request;
using Mapster;
using KASHOP.DAL.Models;
using System.Linq.Expressions;

namespace KASHOP.BLL.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository) {
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryResponse> CreateCategory(CategoryRequest request)
        {
           var category = request.Adapt<Category>();
            await _categoryRepository.CreateAsync(category);
            return category.Adapt<CategoryResponse>();
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetOne(c => c.Id == id);
            if(category == null) return false;
            return await _categoryRepository.DeleteAsync(category);
            
        }

        public async Task<List<CategoryResponse>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllAsync(new string[] {nameof(Category.Translations),
            nameof(Category.CreatedBy)
            });

            return categories.Adapt<List<CategoryResponse>>();

        }

        public async Task<CategoryResponse?> GetCategory(Expression<Func<Category, bool>> filter)
        {
            var category = await _categoryRepository.GetOne(filter, new string[] { nameof(Category.Translations) });
            if (category == null)
                return null;
            return category.Adapt<CategoryResponse>();
        }


    }
}
