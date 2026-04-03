using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using System.Linq.Expressions;

namespace KASHOP.BLL.Service
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetAllCategories();
        Task<CategoryResponse> CreateCategory(CategoryRequest request);
        Task<CategoryResponse?> GetCategory(Expression<Func<Category, bool>> filter);

        Task<bool> DeleteCategory(int id);



    }
}
