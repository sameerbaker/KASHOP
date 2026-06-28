using KASHOP.DAL.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Request
{
    public class ProductRequest
    {
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        [AllowedExtensions]
        [MaxFileSize(5)] // 5MB
        public IFormFile MainImage { get; set; }
        public List<IFormFile> SubImages { get; set; } = new List<IFormFile>();
        public int Quantity { get; set; }
        public List<ProductTranslationRequest> Translations { get; set; } = new List<ProductTranslationRequest>();
        public int CategoryId { get; set; }
    }
}
