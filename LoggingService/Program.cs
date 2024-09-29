using EasyNetQ;
using LoggingService.Services;

var builder = WebApplication.CreateBuilder(args);

// Get RabbitMQ and Database connection details from environment variables
var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitMqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitMqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var dbPass = Environment.GetEnvironmentVariable("DB_PASS") ?? "SuperSecret7!";

// Construct RabbitMQ connection string
var rabbitMqConnectionString = $"host={rabbitMqHost};port={rabbitMqPort};username={rabbitMqUser};password={rabbitMqPass}";

// Register services
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddSingleton<Database>(serviceProvider => new Database(dbHost, dbUser, dbPass)); // Pass DB details here
builder.Services.AddSingleton<IBus>(bus => RabbitHutch.CreateBus(rabbitMqConnectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var logService = scope.ServiceProvider.GetRequiredService<ILogService>();
    MessageSubscriber.StartSubscribing(logService);
}

app.Run();