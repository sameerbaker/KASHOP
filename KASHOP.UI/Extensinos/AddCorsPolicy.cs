using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KASHOP.UI.Extensinos
{
    public static class AddCorsPolicy
    {
        public const string PolicyName = "_myAllowSpecificOrigins";
        public static IServiceCollection AddCorsPolicyServices(this IServiceCollection Services)
        {
           
            Services.AddCors(options =>
            {
                options.AddPolicy(name: PolicyName,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                                  });
            });

            return Services;
        }
    }
}
