using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using SharedDomain.Entities;
using SharedDomain;

namespace PartnerApp.BackgroundJobs
{
    public class PersonCreatedJob : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory scopeFactory;
        private const string Queue = "xpto-partner/person-created";

        public PersonCreatedJob(IServiceScopeFactory scopeFactory)
        {
            _connection = new ConnectionFactory { HostName = "broker_rabbitmq" }.CreateConnection("xpto-partner-person-created-consumer");
            _channel = _connection.CreateModel();
            this.scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                PersonEntity message = JsonConvert.DeserializeObject<PersonEntity>(contentString) ?? default!;

                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.People.AddAsync(message);
                await dbContext.SaveChangesAsync();
                
                Console.WriteLine($"dados recebidos: {message}");

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(Queue, false, consumer);

            return Task.CompletedTask;
        }
    }
}
