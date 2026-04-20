

using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Realtime.Api.Hubs;
using Realtime.Api.Messaging;

namespace Realtime.Api.Worker;

public sealed class BackgroundWorker(IServiceScopeFactory scopeFactory, IHubContext<AeroHub> hub) : BackgroundService
{
      protected async override Task ExecuteAsync(CancellationToken stoppingToken)
      {
           
            while (!stoppingToken.IsCancellationRequested)
            {
                   Console.WriteLine("Running....");
                  // Message Broker
                  using var scope = scopeFactory.CreateScope();
                  var factory = scope.ServiceProvider.GetRequiredService<IRabbitMqFactory>();
                  var connection = await factory.GetConnectionAsync();
                  var channel = await connection.CreateChannelAsync();

                  await channel.ExchangeDeclareAsync("sentrix-exchange", ExchangeType.Topic, true);

                  await channel.QueueDeclareAsync("realtime-queue", true, false, false);

                  await channel.QueueBindAsync(
                      queue: "realtime-queue",
                      exchange: "sentrix-exchange",
                      routingKey: "realtime");

                  var consumer = new AsyncEventingBasicConsumer(channel);

                  consumer.ReceivedAsync += async (sender, ea) =>
                  {
                        try
                        {
                              //   var message = MessageHelper.Deserialize<Event>(ea.Body.ToArray());

                              Console.WriteLine("Receive");
                              await hub.Clients.All.SendAsync("test","123");

                              await Task.Delay(500);

                              await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                              Console.WriteLine($"Error: {ex.Message}");

                              await channel.BasicNackAsync(ea.DeliveryTag, false, false); // requeue
                        }
                  };

                  await channel.BasicConsumeAsync("realtime-queue", false, consumer);

                  // keep worker alive forever
                  await Task.Delay(Timeout.Infinite, stoppingToken);

            }
      }
}
