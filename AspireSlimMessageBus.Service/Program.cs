namespace AspireSlimMessageBus.Service;

using System;
using System.Threading.Tasks;
using AspireSlimMessageBus.Service.Bootstrap;
using AspireSlimMessageBus.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // https://learn.microsoft.com/en-us/dotnet/aspire/messaging/azure-service-bus-integration?tabs=dotnet-cli#tracing
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.AddSlimMessaging("messaging");
        builder.Services.AddHostedService<MessageProducerService>();

        var app = builder.Build();
        app.MapDefaultEndpoints();
        await app.RunAsync();
    }
}
