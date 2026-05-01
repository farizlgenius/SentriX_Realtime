

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Realtime.Api.Interfaces;

namespace Realtime.Api.Messaging;


public sealed class RabbitMqFactory : IRabbitMqFactory
{
      private readonly ConnectionFactory _factory;
      private IConnection? _connection;
      private readonly object _lock = new();

      public RabbitMqFactory(IRabbitMqOption settings)
      {

            _factory = new ConnectionFactory
            {
                  HostName = settings.Host,
                  Port = settings.Port,
                  UserName = settings.Username,
                  Password = settings.Password,
                  VirtualHost = "/",
                  //DispatchConsumersAsync = true,
                  RequestedConnectionTimeout = TimeSpan.FromSeconds(10),
                  SocketReadTimeout = TimeSpan.FromSeconds(10),
                  SocketWriteTimeout = TimeSpan.FromSeconds(10)
            };
      }

      public void Dispose()
      {
            if (_connection?.IsOpen == true)
                  _connection.Dispose();
      }

      public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
      {
            if (_connection != null && _connection.IsOpen)
                  return _connection;

            lock (_lock)
            {
                  if (_connection != null && _connection.IsOpen)
                        return _connection;
            }

            try
            {
                  _connection = await _factory.CreateConnectionAsync(cancellationToken);
                  Console.WriteLine("Connected");
                  return _connection;
            }
            catch (BrokerUnreachableException ex)
            {
                  throw new Exception("RabbitMQ unreachable", ex);
            }
      }
}
