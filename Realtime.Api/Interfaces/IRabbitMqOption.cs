using System;

namespace Realtime.Api.Interfaces;

public interface IRabbitMqOption
{
  string Host { get; }
  int Port { get; }
  string Username { get; }
  string Password { get; }
}
