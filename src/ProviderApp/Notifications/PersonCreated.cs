using Flurl;
using Flurl.Http;
using MediatR;
using SharedDomain.Entities;

namespace ProviderApp.Notifications
{
    public class PersonCreated(PersonEntity person) : INotification
    {
        public PersonEntity Person { get; } = person;
    }

    public class PersonCreatedNotification(ILogger<PersonCreatedNotification> logger) : INotificationHandler<PersonCreated>
    {
        private readonly ILogger<PersonCreatedNotification> logger = logger;

        public async Task Handle(PersonCreated notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("A pessoa {name} foi criada no sistema! ", notification.Person.Name);

            string uri_notification_consumer = Environment.GetEnvironmentVariable("URI_NOTIFICATION_CONSUMER")!;

            var jobId = await uri_notification_consumer
                .AppendPathSegment("person-created")
                .WithHeader("Content-Type", "application/json")
                .PostJsonAsync(notification.Person, cancellationToken: cancellationToken)
                .ReceiveString();

            logger.LogInformation("O NotificationConsumer recebeu {name} e executou a fila {jobId}", notification.Person.Name, jobId);
        }
    }
}
