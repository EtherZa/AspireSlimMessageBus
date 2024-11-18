namespace AspireSlimMessageBus.Service.Bootstrap
{
    using System;
    using AspireSlimMessageBus.Service.Consumers;
    using AspireSlimMessageBus.Service.Models;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SlimMessageBus;
    using SlimMessageBus.Host;
    using SlimMessageBus.Host.AzureServiceBus;
    using SlimMessageBus.Host.Serialization.SystemTextJson;

    public static class Messaging
    {
        public static T AddSlimMessaging<T>(this T builder, string connectionName)
            where T : IHostApplicationBuilder
        {
            const string queueName = "aspire";

            AddAspire(builder, connectionName, queueName);
            AddServices(builder.Services);
            AddSlimMessageBus(builder.Services, queueName);

            return builder;
        }
        private static void AddAspire(IHostApplicationBuilder builder, string connectionName, string queueName)
        {
            builder.AddAzureServiceBusClient(
                connectionName,
                o =>
                {
                    o.DisableTracing = false;
                    o.HealthCheckQueueName = queueName;
                });

            // https://github.com/Azure-Samples/eShopOnAzure/blob/9da1eccec3454b19cd11a9b44001cd0323deb749/src/EventBusServiceBus/ServiceBusDependencyInjectionExtensions.cs#L33
            // Temporary until https://github.com/dotnet/aspire/issues/431
            var connectionString = builder.Configuration.GetConnectionString(connectionName);
            builder.Services.AddAzureClients(
                o =>
                {
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        // a service bus namespace can't contain ';'. if it is found assume it is a connection string
                        if (!connectionString.Contains(';'))
                        {
                            o.AddServiceBusAdministrationClientWithNamespace(connectionString);
                        }
                        else
                        {
                            o.AddServiceBusAdministrationClient(connectionString);
                        }
                    }
                });
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IConsumer<SampleMessage>, SampleConsumer>();
        }

        private static void AddSlimMessageBus(IServiceCollection services, string queueName)
        {
            services.AddSlimMessageBus(
                bus =>
                {
                    bus.PerMessageScopeEnabled(true);
                    bus.AddJsonSerializer();

                    bus.WithProviderServiceBus(
                        cfg =>
                        {
                            cfg.ConnectionString = "ignored_but_required";

                            cfg.ClientFactory = (sp, _) => sp.GetRequiredService<ServiceBusClient>();
                            cfg.AdminClientFactory = (sp, _) => sp.GetRequiredService<ServiceBusAdministrationClient>();

                            cfg.TopologyProvisioning.Enabled = true;
                            cfg.TopologyProvisioning.CanConsumerCreateQueue = true;
                            cfg.TopologyProvisioning.CanProducerCreateQueue = true;
                            cfg.TopologyProvisioning.CreateQueueOptions = o => o.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                        });

                    bus.Produce<SampleMessage>(x => x.DefaultQueue(queueName));
                    bus.Consume<SampleMessage>(x => x.Queue(queueName, c => c.WithConsumer<IConsumer<SampleMessage>>()));
                });
        }
    }
}
