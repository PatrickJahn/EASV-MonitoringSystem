using MonitoringAPI.Services;
using EasyNetQ;

var builder = WebApplication.CreateBuilder(args);

var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitMqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitMqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";

var rabbitMqConnectionString = $"host={rabbitMqHost};port={rabbitMqPort};username={rabbitMqUser};password={rabbitMqPass}";

builder.Services.AddSingleton<IBus>(bus => RabbitHutch.CreateBus(rabbitMqConnectionString));

// Register MessagePublisher and other services
builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();
builder.Services.AddScoped<LogServiceRequester>();
builder.Services.AddSingleton<LogServiceResponseHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure that LogServiceResponseHandler starts consuming log responses
using (var scope = app.Services.CreateScope())
{
    var logResponseHandler = scope.ServiceProvider.GetRequiredService<LogServiceResponseHandler>();
}

app.Run();