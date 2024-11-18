namespace AspireSlimMessageBus.Service.Consumers;

using System;
using System.Threading.Tasks;
using AspireSlimMessageBus.Service.Models;
using Microsoft.Extensions.Logging;
using SlimMessageBus;

public class SampleConsumer : IConsumer<SampleMessage>
{
    private readonly ILogger<SampleConsumer> _logger;
    private readonly IMessageBus _messageBus;

    public SampleConsumer(
        ILogger<SampleConsumer> logger,
        IMessageBus messageBus)
    {
        this._logger = logger;
        this._messageBus = messageBus;
    }

    public async Task OnHandle(SampleMessage message)
    {
        this._logger.LogInformation("Sample message: {Message} - {Depth}", message.Message, message.Depth);

        if (Random.Shared.NextDouble() <= 0.6)
        {
            var child = new SampleMessage(message.Message, message.Depth + 1);
            await this._messageBus.Publish(child);
        }
    }
}
