using Realtime.Api.Hubs;
using Realtime.Api.Interfaces;
using Realtime.Api.Messaging;
using Realtime.Api.Worker;
using Realtime.Infrastructure.Setting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddHostedService<BackgroundWorker>();
builder.Services.AddOptions<RabbitMqOption>()
        .Bind(builder.Configuration.GetSection("RabbitMQ"))
        .ValidateOnStart();
        builder.Services.AddScoped<IRabbitMqFactory,RabbitMqFactory>();
        builder.Services.AddSingleton<IRabbitMqOption>(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOption>>().Value);

var app = builder.Build();

app.MapHub<AeroHub>("/aero");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

