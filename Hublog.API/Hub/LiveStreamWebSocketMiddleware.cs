using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Hublog.Repository.Entities.Model.LivestreamModal;

namespace Hublog.API.Hub
{
    public class LiveStreamWebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly List<WebSocket> _connectedClients = new();

        public LiveStreamWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws/livestream")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    _connectedClients.Add(webSocket);

                    await ReceiveAndBroadcastAsync(webSocket);

                    _connectedClients.Remove(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task ReceiveAndBroadcastAsync(WebSocket senderSocket)
        {
            var buffer = new byte[4096];

            while (senderSocket.State == WebSocketState.Open)
            {
                var result = await senderSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await senderSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    break;
                }

                var jsonPayload = Encoding.UTF8.GetString(buffer, 0, result.Count);

                try
                {
                    var data = JsonSerializer.Deserialize<LivestreamModal>(jsonPayload, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (data != null)
                    {
                        Console.WriteLine($"Server received data: {data.UserId}, {data.OrganizationId}, {data.ActiveApp}");

                        var responseJson = JsonSerializer.Serialize(new
                        {
                            data.UserId,
                            data.OrganizationId,
                            data.ActiveApp,
                            data.ActiveUrl,
                            data.LiveStreamStatus,
                            data.ActiveAppLogo,
                            data.ActiveScreenshot,
                            data.Latitude,
                            data.Longitude
                        });

                        var responseBytes = Encoding.UTF8.GetBytes(responseJson);

                        foreach (var socket in _connectedClients.ToList())
                        {
                            if (socket.State == WebSocketState.Open)
                            {
                                await socket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing payload: {ex.Message}");
                }
            }
        }
    }
}
