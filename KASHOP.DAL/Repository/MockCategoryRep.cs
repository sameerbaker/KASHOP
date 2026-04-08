using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repository
{
    internal class MockCategoryRep : ICategoryRepository
    {
        public Task<Category> CreateAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Category>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Category>> GetAllAsync(Expression<Func<Category, bool>> filter = null, string[]? includes = null)
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetOne(Expression<Func<Category, bool>> filter, string[]? includes = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Category entity)
        {
            throw new NotImplementedException();
        }
    }
}
