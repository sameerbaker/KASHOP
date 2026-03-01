using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    public class CategoryTranslation
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string Language { get; set; } = "en";

        //[JsonIgnore]
        public Category Catagory { get; set; }

        public int CatagoryId { get; set; }
    }
}
