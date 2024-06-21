using Hangfire;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SharedDomain.Entities;
using System.Text;

namespace NotificationConsumer.PersonJobs
{
    public class PersonValidateAndPublishJobs
    {
        private readonly ILogger<PersonValidateAndPublishJobs> logger;

        private const string exchange_name = "provider-service";
        private const string routingKey_created = "person-created";
        private const string routingKey_updated = "person-updated";
        private const string routingKey_inactivated = "person-inactivated";

        private readonly IModel _channel;

        public PersonValidateAndPublishJobs(ILogger<PersonValidateAndPublishJobs> logger)
        {
            this.logger = logger;
            IConnection _connection = new ConnectionFactory { HostName = "broker_rabbitmq" }.CreateConnection("provider-service-publisher");
            _channel = _connection.CreateModel();
        }

        [Queue("person_created")]
        public Task PersonCreated(PersonEntity personEntity)
        {
            ArgumentNullException.ThrowIfNull(personEntity);

            logger.LogInformation("" +
                "{@person} recebida no consumidor interno. " +
                "Agora será aplicado regras de negócios para garantir que somente os assinantes corretos recebam o dado!"
                , personEntity);

            if (personEntity.CanItBeShared)
            {
                try
                {
                    var payload = JsonConvert.SerializeObject(personEntity);
                    var byteArray = Encoding.UTF8.GetBytes(payload);
                    _channel.BasicPublish(exchange_name, routingKey_created, null, byteArray);
                    logger.LogInformation("{@person} publicado!", personEntity);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Não foi possível fazer a publicação do evento!");
                    throw new InvalidOperationException("falha na publicação.", ex);
                }
            }

            return Task.CompletedTask;
        }

        [Queue("person_updated")]
        public Task PersonUpdated(PersonUpdatedRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            logger.LogInformation("" +
                "{@request} recebida no consumidor interno. " +
                "Agora será aplicado regras de negócios para garantir que somente os assinantes corretos recebam o dado!"
                , request);

            var old = request.OldPerson;
            var updated = request.UpdatedPerson;

            string routingKey;

            ///Já compartilhava, e continua compartilhando, então é uma atualização para o partner;
            if (old.CanItBeShared && updated.CanItBeShared)
            {
                routingKey = routingKey_updated;
            }
            ///Não compartilhava, e agora compartilha, então é uma criação para o partner;
            else if (!old.CanItBeShared && updated.CanItBeShared)
            {
                routingKey = routingKey_created;
            }
            ///Compartilhava, e agora não compartilha mais, então é uma inativação para o partner;
            else if (old.CanItBeShared && !updated.CanItBeShared)
            {
                routingKey = routingKey_inactivated;
            }
            ///Não compartilhava, e continua não compartilhando, então ignora e sai;
            else
            {
                return Task.CompletedTask;
            }

            try
            {
                var payload = JsonConvert.SerializeObject(request.UpdatedPerson);
                var byteArray = Encoding.UTF8.GetBytes(payload);
                _channel.BasicPublish(exchange_name, routingKey, null, byteArray);
                logger.LogInformation("{@request} publicado!", request);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Não foi possível fazer a publicação do evento!");
                throw new InvalidOperationException("falha na publicação.", ex);
            }

            return Task.CompletedTask;
        }

        [Queue("person_inactivated")]
        public Task PersonInactivated(PersonEntity personEntity)
        {
            ArgumentNullException.ThrowIfNull(personEntity);

            logger.LogInformation("" +
                "{@person} recebida no consumidor interno. " +
                "Agora será aplicado regras de negócios para garantir que somente os assinantes corretos recebam o dado!"
                , personEntity);

            if (personEntity.CanItBeShared)
            {
                try
                {
                    var payload = JsonConvert.SerializeObject(personEntity);
                    var byteArray = Encoding.UTF8.GetBytes(payload);
                    _channel.BasicPublish(exchange_name, routingKey_inactivated, null, byteArray);
                    logger.LogInformation("{@person} publicado!", personEntity);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Não foi possível fazer a publicação do evento!");
                    throw new InvalidOperationException("falha na publicação.", ex);
                }
            }

            return Task.CompletedTask;
        }
    }
}
