namespace AspireSlimMessageBus.Service.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using AspireSlimMessageBus.Service.Models;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SlimMessageBus;

    public class MessageProducerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;

        public MessageProducerService(
            ILogger<MessageProducerService> logger,
            IMessageBus messageBus)
        {
            this._logger = logger;
            this._messageBus = messageBus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var i = 0;
            while (!stoppingToken.IsCancellationRequested && i < 3)
            {
                var message = new SampleMessage($"Message {i++}");
                await this._messageBus.Publish(message, cancellationToken: stoppingToken);

                this._logger.LogInformation("Message sent: {Message}", message.Message);

                await Task.Delay(3000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
