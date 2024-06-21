using Flurl;
using Flurl.Http;
using MediatR;
using SharedDomain.Entities;

namespace ProviderApp.Notifications
{
    public class PersonInactivated(PersonEntity person) : INotification
    {
        public PersonEntity Person { get; } = person;
    }

    public class PersonInactivatedNotification(ILogger<PersonInactivatedNotification> logger) : INotificationHandler<PersonInactivated>
    {
        private readonly ILogger<PersonInactivatedNotification> logger = logger;

        public async Task Handle(PersonInactivated notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Inativação de pessoa! {@notification}", notification);

            string uri_notification_consumer = Environment.GetEnvironmentVariable("URI_NOTIFICATION_CONSUMER")!;

            var jobId = await uri_notification_consumer
                .AppendPathSegment("person-inactivated")
                .WithHeader("Content-Type", "application/json")
                .PutJsonAsync(notification.Person, cancellationToken: cancellationToken)
                .ReceiveString();

            logger.LogInformation("O NotificationConsumer recebeu {@person} e executou a fila {jobId}", notification.Person, jobId);
        }
    }
}
