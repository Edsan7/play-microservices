using System;
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Service.Settings;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {

            services.AddMassTransit((configuration) =>
            {
                configuration.AddConsumers(Assembly.GetEntryAssembly());

                configuration.UsingRabbitMq((context, configure) =>
                {
                    var configuration = context.GetService<IConfiguration>();
                    var serviceSettigs = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var rabbitSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    configure.Host(rabbitSettings.Host);
                    configure.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettigs.ServiceName, false));
                    configure.UseMessageRetry(retryConfigurator =>
                    {
                        retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });

            return services;

        }
    }
}