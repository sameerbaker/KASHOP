using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System.Linq.Expressions;


namespace KASHOP.BLL.Service
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }
        public async Task<BrandResponse> CreateBrand(BrandRequest request)
        {
            var brand = request.Adapt<Brand>();
            await _brandRepository.CreateAsync(brand);
            return brand.Adapt<BrandResponse>();
        }
        public async Task<bool> DeleteBrand(int id)
        {
            var brand = await _brandRepository.GetOne(b => b.Id == id);
            if (brand == null) return false;
            return await _brandRepository.DeleteAsync(brand);
        }
        public async Task<List<BrandResponse>> GetAllBrands()
        {
            var brands = await _brandRepository.GetAllAsync(null ,new string[] { nameof(Brand.CreatedBy) });
            return brands.Adapt<List<BrandResponse>>();
        }
        public async Task<BrandResponse?> GetBrand(Expression<Func<Brand, bool>> filter)
        {
            var brand = await _brandRepository.GetOne(filter);
            if (brand == null) return null;
            return brand.Adapt<BrandResponse>();
        }
    }
}
