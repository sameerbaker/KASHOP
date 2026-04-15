
namespace KASHOP.UI.Extensinos
{
    public static class BuilderExtensinos
    {
        public static IServiceCollection AddBuilderServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            //Database connection
            Services.AddDatabaseServices(Configuration);

            // Add services to the container.
            Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            Services.AddOpenApi();
            //add cors policy
            Services.AddCorsPolicyServices();
            //Add localization services
            Services.AddlocalizationServices();
            // Add application services
            Services.AddApplicationServices();
            // Add Identity services
            Services.AddIdentityServices();
            // Add authentication services
            Services.AddJWTAuthenticationServices(Configuration);
            Services.AddAuthentication();

            return Services;
        }
    }
}
