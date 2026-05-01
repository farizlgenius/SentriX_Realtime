using System;
using Realtime.Api.DTOs;

namespace Realtime.Api.Interfaces;

public interface IRabbitMqHandler
{
      Task HandleAsync(UiDto message,CancellationToken ct = default);
}
