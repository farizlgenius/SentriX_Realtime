using System;
using Microsoft.AspNetCore.SignalR;
using Realtime.Api.Constants;
using Realtime.Api.DTOs;
using Realtime.Api.Hubs;
using Realtime.Api.Interfaces;

namespace Realtime.Api.Handler;

public class MessageHandler(IHubContext<SentriXHub> hub) : IRabbitMqHandler
{

      public async Task HandleAsync(UiDto dto, CancellationToken ct = default)
      {
            await hub.Clients.All.SendAsync(dto.Key,dto.Data);
      }
}
