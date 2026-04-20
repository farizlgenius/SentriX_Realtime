using System.Text;
using System.Text.Json;

public static class MessageHelper
{
  public static byte[] Serialize<T>(T obj)
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));

  public static T Deserialize<T>(byte[] body)
      {
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Deserialized message: {message}");
            return JsonSerializer.Deserialize<T>(message)!;
      }

}