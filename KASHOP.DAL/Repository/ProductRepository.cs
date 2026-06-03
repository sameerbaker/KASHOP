using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> DecreaseQuantityAsync(int productId, int amount)
        {
            var product = await GetOne(p => p.Id == productId);
            if (product.Quantity < amount) return false;
            product.Quantity -= amount;
            await UpdateAsync(product);
            return product.Quantity < 5; // Return true if quantity is low
        }
    }
}
