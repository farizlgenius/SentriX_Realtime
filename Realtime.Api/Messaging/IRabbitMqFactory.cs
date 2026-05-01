using System;
using RabbitMQ.Client;

namespace Realtime.Api.Messaging;

public interface IRabbitMqFactory : IDisposable
{
      Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
}
