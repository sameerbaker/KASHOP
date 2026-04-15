using KASHOP.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace KASHOP.UI.Extensinos
{
    public static class DatabaseExtensinos
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnention"));
            });
            return Services;

        }
    }
}
