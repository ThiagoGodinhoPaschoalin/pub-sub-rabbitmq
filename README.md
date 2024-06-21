# Shared data between systems using Publish/Subscribe

## Execute 

<br />

> Create network
```pwsh
docker network create -d bridge shareddatawithpubsub_net
```

<br />

> Start projects
```pwsh
docker compose -f .\src\docker-compose.yml -f .\src\docker-compose.override.yml up -d
```

1. Open `Data provider site` (http://localhost:8071/)
1. Create new Person and save
1. Now open `xpto partner site` (http://localhost:8081) and the same person will be registered.


# Documentation

## RabbitMq:

### Basic configuration in Broker:

1. Open http://localhost:15672/
1. Login/Password: `guest`
1. GoTo `Exchanges` -> `Add a new exchange`:
	1. Name: `provider-service`
	1. Type: `topic`
	1. Click on `Add exchange`;
1. GoTo `Queues` -> `Add a new queue`:
	1. Name: `xpto-partner/person-created`
	1. Type: `Classic`
	1. Click on `Add queue`;
1. Click on `xpto-partner/person-created` queue, in menu `bindings`:
	1. From exchange: `provider-service`
	1. Routing key: `person-created`

> * `xpto-partner` is a generic company name 

### Basic configuration in c# publisher:

```bash
dotnet add package RabbitMQ.Client
```

```csharp
using RabbitMQ.Client;

string rabbit_domain = "broker_rabbitmq"; //with docker-compose, hostname is the container_name, otherwise `localhost`;
string exchange_name = "provider-service";

var factory = new ConnectionFactory { HostName = rabbit_domain };
IConnection _connection = factory.CreateConnection($"{exchange_name}-publisher");
IModel _channel = _connection.CreateModel();

string message = "Hello World, partner!";
var byteArray = Encoding.UTF8.GetBytes(message);

_channel.BasicPublish("provider-service", "person-created", null, byteArray);
```

### Basic configuration in C# subscribe:

```bash
dotnet add package RabbitMQ.Client
```

```csharp
///PersonCreatedJob.cs

public class PersonCreatedJob : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string queue_name;

    public PersonCreatedJob()
    {
        string rabbit_domain = "broker_rabbitmq";//with docker-compose, hostname is the container_name, otherwise `localhost`;
        queue_name = "xpto-partner/person-created";
        string client_provided_name = $"{queue_name.Replace("/", "-")}-consumer";

        var factory = new ConnectionFactory { HostName = rabbit_domain };
        _connection = factory.CreateConnection(client_provided_name);
        _channel = _connection.CreateModel();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, eventArgs) =>
        {
            var contentArray = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(contentArray);

            Console.WriteLine($"Mensagem recebida: {message}");

            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };

        _channel.BasicConsume(queue_name, false, consumer);

        return Task.CompletedTask;
    }
}
```

```csharp
//Program.cs

builder.Services.AddHostedService<PersonCreatedJob>();
```


# References

* https://www.rabbitmq.com/
* https://www.hangfire.io/
* https://learn.microsoft.com/pt-br/aspnet/core/razor-pages/?view=aspnetcore-8.0&tabs=visual-studio
* https://learn.microsoft.com/pt-br/ef/core/
* https://github.com/jbogard/MediatR/wiki