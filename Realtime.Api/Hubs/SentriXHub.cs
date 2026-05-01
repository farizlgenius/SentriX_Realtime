using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Realtime.Api.Hubs;

[Authorize]
public sealed class SentriXHub : Hub
{

}
