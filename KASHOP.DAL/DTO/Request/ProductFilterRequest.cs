using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Request
{
    public class ProductFilterRequest : PaginationRequest
    {
        public string? Name { get; set; }
        public int? CategoryId { get; set; }
        //public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinRate { get; set; }
        public double? MaxRate { get; set; }


    }
}
