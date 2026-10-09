using CoreService.Domain.Interfaces;
using CoreService.Infrastructure.Data;
using CoreService.Infrastructure.Repositories;
using CoreService.Infrastructure.Consumers;
using MassTransit; // <-- Adicione este using
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configuração do Banco de Dados (EF Core + Postgres)
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // 2. Configuração de Mensageria (MassTransit + RabbitMQ)
        services.AddMassTransit(x =>
        {

            x.AddConsumer<ReceiptProcessedConsumer>();
        
            // Define o RabbitMQ como o "transportador"
            x.UsingRabbitMq((context, cfg) =>
            {
                // Busca a URL de conexão do RabbitMQ nas configurações
                var rabbitMqConnection = configuration.GetConnectionString("RabbitMqConnection")
                    ?? "amqp://guest:guest@localhost:5672";

                cfg.Host(rabbitMqConnection);

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}