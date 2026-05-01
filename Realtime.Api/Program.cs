using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Realtime.Api.Handler;
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
    options =>
    {
        var secret = "wVmLc8GTnNC7CeoLE27dp6bkm5drf8hHZJpzyf82jpQ=";
        var key = Encoding.UTF8.GetBytes(secret);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "SentriX",

            ValidateAudience = true,
            ValidAudience = "SentriXUsers",

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["accessToken"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrWhiteSpace(accessToken) &&
                    path.StartsWithSegments("/aeroHubs")
                )
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };

    }
);
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
builder.Services.AddHostedService<RabbitMqWorker>();
builder.Services.AddOptions<RabbitMqOption>()
        .Bind(builder.Configuration.GetSection("RabbitMQ"))
        .ValidateOnStart();
builder.Services.AddScoped<IRabbitMqFactory, RabbitMqFactory>();
builder.Services.AddScoped<IRabbitMqHandler,MessageHandler>();
builder.Services.AddSingleton<IRabbitMqOption>(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOption>>().Value);

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("cors");
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<SentriXHub>("/sentrixHubs");



app.Run();

