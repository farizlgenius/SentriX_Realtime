using System;
using Realtime.Api.Interfaces;

namespace Realtime.Infrastructure.Setting;

public sealed class RabbitMqOption : IRabbitMqOption
{
  public string Host { get; set; } = string.Empty;
  public int Port { get; set; }
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

}

