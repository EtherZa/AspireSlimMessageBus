namespace AspireSlimMessageBus.AppHost;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        const string queueName = "aspire";

        var serviceBus = builder.ExecutionContext.IsPublishMode
            ? builder.AddAzureServiceBus("messaging").AddQueue(queueName)
            : builder.AddConnectionString("messaging");

        builder.AddProject<Projects.AspireSlimMessageBus_Service>("messageService")
            .WithExternalHttpEndpoints()
            .WithEnvironment("queueName", queueName)
            .WithReference(serviceBus);

        await builder.Build().RunAsync();
    }
}