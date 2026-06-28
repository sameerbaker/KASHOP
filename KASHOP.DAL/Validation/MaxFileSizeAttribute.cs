using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Validation
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxSizeInMB;
        public MaxFileSizeAttribute(long maxSizeInMB)
        {
            _maxSizeInMB = maxSizeInMB;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxSizeInMB * 1024 * 1024) // 2MB
                {
                    return new ValidationResult($"File size should not exceed : {_maxSizeInMB}MB.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
