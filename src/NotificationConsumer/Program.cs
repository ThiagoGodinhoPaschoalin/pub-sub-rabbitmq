using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Mvc;
using NotificationConsumer;
using NotificationConsumer.PersonJobs;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using SharedDomain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context,config) => 
{
    config.WriteTo.Console(LogEventLevel.Debug, "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
    config.Enrich.FromLogContext().Enrich.WithExceptionDetails();
});



#region HANGFIRE

builder.Services.AddHangfire( (serviceProvider, global) => global
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSerilogLogProvider()
    .UseConsole()
    .UseMemoryStorage(new MemoryStorageOptions
    {
        CountersAggregateInterval = TimeSpan.FromMinutes(5),
        FetchNextJobTimeout = TimeSpan.FromMinutes(5),
        JobExpirationCheckInterval = TimeSpan.FromMinutes(5)
    })
    .WithJobExpirationTimeout(TimeSpan.FromHours(6))
);

builder.Services.AddHangfireServer(options =>
{
    options.CancellationCheckInterval = TimeSpan.FromMinutes(5);
    options.HeartbeatInterval = TimeSpan.FromMinutes(1);
    options.ServerCheckInterval = TimeSpan.FromMinutes(1);
    options.WorkerCount = 1;
    options.ServerName = "generic";
    options.Queues = ["default"];
});

builder.Services.AddHangfireServer(options =>
{
    options.CancellationCheckInterval = TimeSpan.FromMinutes(5);
    options.HeartbeatInterval = TimeSpan.FromMinutes(1);
    options.ServerCheckInterval = TimeSpan.FromMinutes(1);
    options.WorkerCount = 10;
    options.ServerName = "person";
    options.Queues = ["person_created", "person_updated", "person_inactivated"];
});

#endregion

builder.Services.AddTransient<PersonValidateAndPublishJobs>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseHangfireDashboard(options: new DashboardOptions { Authorization = [ new MyAuthorizationFilter() ] });

BackgroundJob.Enqueue(() => MinhasFuncoes.Inicializacao());

//RecurringJob.AddOrUpdate(
//    "IdDoJobRecorrenteDeOla",
//    () => MinhasFuncoes.RecorrenciaNaFila001(),
//    "* * * * *");
///https://crontab.guru

#region Controllers

app.MapGet("/check", () => { return "OK"; });

app.MapPost("/notifications/person-created", ([FromBody] PersonEntity request, [FromServices] PersonValidateAndPublishJobs jobs) =>
{
    BackgroundJob.Enqueue(() => jobs.PersonCreated(request));
})
.WithName("ReceiveNotifyAboutPersonCreated")
.AllowAnonymous()
.WithOpenApi();

app.MapPut("/notifications/person-updated", ([FromBody] PersonUpdatedRequest request, [FromServices] PersonValidateAndPublishJobs jobs) =>
{
    BackgroundJob.Enqueue(() => jobs.PersonUpdated(request));
})
.WithName("ReceiveNotifyAboutPersonUpdated")
.AllowAnonymous()
.WithOpenApi();

app.MapPut("/notifications/person-inactivated", ([FromBody] PersonEntity request, [FromServices] PersonValidateAndPublishJobs jobs) =>
{
    BackgroundJob.Enqueue(() => jobs.PersonInactivated(request));
})
.WithName("ReceiveNotifyAboutPersonInactivated")
.AllowAnonymous()
.WithOpenApi();

#endregion

app.Run();


public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}

public record PersonUpdatedRequest(PersonEntity OldPerson, PersonEntity UpdatedPerson);