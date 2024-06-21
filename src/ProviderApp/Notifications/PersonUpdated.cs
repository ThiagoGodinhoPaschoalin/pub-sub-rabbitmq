using Flurl;
using Flurl.Http;
using MediatR;
using SharedDomain.Entities;

namespace ProviderApp.Notifications
{
    public class PersonUpdated(PersonEntity old, PersonEntity updated) : INotification
    {
        public PersonEntity OldPerson { get; } = old;
        public PersonEntity UpdatedPerson { get; } = updated;
    }

    public class PersonUpdatedNotification(ILogger<PersonUpdatedNotification> logger) : INotificationHandler<PersonUpdated>
    {
        private readonly ILogger<PersonUpdatedNotification> logger = logger;

        public async Task Handle(PersonUpdated notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Atualização no sistema! {@notification}", notification);

            string uri_notification_consumer = Environment.GetEnvironmentVariable("URI_NOTIFICATION_CONSUMER")!;

            var jobId = await uri_notification_consumer
                .AppendPathSegment("person-updated")
                .WithHeader("Content-Type", "application/json")
                .PutJsonAsync(notification, cancellationToken: cancellationToken)
                .ReceiveString();

            logger.LogInformation("O NotificationConsumer recebeu {id} e executou a fila {jobId}", notification.OldPerson.Id, jobId);
        }
    }
}
