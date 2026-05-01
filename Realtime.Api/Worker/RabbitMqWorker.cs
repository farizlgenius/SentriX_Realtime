

using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Realtime.Api.Constants;
using Realtime.Api.DTOs;
using Realtime.Api.Hubs;
using Realtime.Api.Interfaces;
using Realtime.Api.Messaging;

namespace Realtime.Api.Worker;

public sealed class RabbitMqWorker(IServiceScopeFactory scopeFactory) : BackgroundService
{
      protected async override Task ExecuteAsync(CancellationToken ct)
      {
           
            while (!ct.IsCancellationRequested)
            {
                  // Message Broker
                  using var scope = scopeFactory.CreateScope();
                  var factory = scope.ServiceProvider.GetRequiredService<IRabbitMqFactory>();
                  var handlers = scope.ServiceProvider.GetRequiredService<IRabbitMqHandler>();
                  var connection = await factory.GetConnectionAsync();
                  var channel = await connection.CreateChannelAsync();

                  await channel.BasicQosAsync(0, 50, false);

                  await channel.ExchangeDeclareAsync(RabbitMqConstant.EXCHANGE,ExchangeType.Topic,true);

                  await channel.QueueDeclareAsync(RabbitMqConstant.QUEUE,true,false,false);

                  await channel.QueueBindAsync(RabbitMqConstant.QUEUE, RabbitMqConstant.EXCHANGE, "#");


                  var consumer = new AsyncEventingBasicConsumer(channel);

                  consumer.ReceivedAsync += async (_, ea) =>
                  {
                        try
                        {
                              var message = MessageHelper.Deserialize<UiDto>(ea.Body.ToArray());
                              // var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                              Console.WriteLine(message.Key);
                              Console.WriteLine(message.Data);

                              await handlers.HandleAsync(message,ct);

                              await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                              Console.WriteLine($"Error: {ex.Message}");

                              await channel.BasicNackAsync(ea.DeliveryTag, false, false); // requeue
                        }
                  };


                  await channel.BasicConsumeAsync(RabbitMqConstant.QUEUE, false, consumer);
                        

                  // keep worker alive forever
                  await Task.Delay(Timeout.Infinite, ct);

            }
      }
}
