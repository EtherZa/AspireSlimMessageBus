# Spike: SlimMessageBus running with Aspire / Azure Service Bus

Spike will produce 3 seed messages which then each have a 60% chance of self-propagating. After which, no further messages will be produced, but SMB will continue to listen to messages.

*Add ASB connection string to AspireSlimMessageBus.AppHost/appsettings.json (ConnectionStrings > messaging)*

## Integration:
1. Configure `ServiceBusMessageSettings.ClientFactory` to retrieve the service from the service provider.
2. Supply a `ServiceBusMessageSettings.ConnectionString` with a non-null/empty value to pass validation.

## Topology creation:
1. Configure `ServiceBusMessageSettings.AdminClientFactory` to retrieve the service from the service provider.
2. Manually add the `ServiceBusAdministrationClient` for topology creation.

## Tracing:
1. Execute `AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);` on application startup as per https://learn.microsoft.com/en-us/dotnet/aspire/messaging/azure-service-bus-integration?tabs=dotnet-cli#tracing.

## Metrics:
Not supported by Azure SDK for .net (https://learn.microsoft.com/en-us/dotnet/aspire/messaging/azure-service-bus-integration?tabs=dotnet-cli#metrics).