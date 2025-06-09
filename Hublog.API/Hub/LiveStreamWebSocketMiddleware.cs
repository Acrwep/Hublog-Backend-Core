using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hublog.API.Hub
{
    public class LiveStreamWebSocketMiddleware
    {
        private static readonly object _clientsLock = new();
        private static readonly List<WebSocket> _connectedClients = new();

        private readonly RequestDelegate _next;

        public LiveStreamWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Equals("/ws/livestream", StringComparison.OrdinalIgnoreCase))
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine($"WebSocket connected: {context.Connection.RemoteIpAddress}");

                    lock (_clientsLock)
                    {
                        _connectedClients.Add(webSocket);
                    }

                    try
                    {
                        await HandleWebSocketConnection(webSocket);
                    }
                    finally
                    {
                        lock (_clientsLock)
                        {
                            _connectedClients.Remove(webSocket);
                        }
                        await TryCloseSocket(webSocket);
                        Console.WriteLine($"WebSocket disconnected: {context.Connection.RemoteIpAddress}");
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            var buffer = new byte[4096];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await TryCloseSocket(webSocket);
                    break;
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message from client: {message}");

                    // For demo, just broadcast back the same message with type metadata
                    var response = new
                    {
                        type = "metadata",
                        userId = 123,
                        activeApp = "DemoApp",
                        timestamp = DateTime.UtcNow.ToString("O")
                    };
                    string json = JsonSerializer.Serialize(response);
                    var bytes = Encoding.UTF8.GetBytes(json);

                    await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        private async Task TryCloseSocket(WebSocket socket)
        {
            if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }
    }
}
