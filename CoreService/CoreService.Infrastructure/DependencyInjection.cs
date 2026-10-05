using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Aqui serão registrados os serviços da camada de infraestrutura (ex: Repositórios, DbContext, etc.)
            
            return services;
        }
    }
}
