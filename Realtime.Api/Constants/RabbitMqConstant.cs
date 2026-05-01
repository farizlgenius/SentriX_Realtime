using System;

namespace Realtime.Api.Constants;

public class RabbitMqConstant
{
      public static string EXCHANGE = "ui.events.exchange";
      public static string QUEUE = "realtime.signalr.queue";
      public static string ROUTING_KEY = "ui.#";
}
