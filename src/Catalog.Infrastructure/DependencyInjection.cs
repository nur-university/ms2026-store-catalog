using Catalog.Infrastructure.Extensions;
using Joseco.Communication.External.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nur.Store2026.Security;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string serviceName)
    {
        services.AddSecrets(configuration, environment)
            .AddObservability(environment, serviceName)
            .AddDatabase()
            .AddSecurity(environment)
            .AddRabbitMQ();

        return services;
    }

}
