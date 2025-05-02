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
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

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
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    _connectedClients.Add(webSocket);
                    
                    try
                    {
                        await HandleWebSocketConnection(webSocket);
                    }
                    finally
                    {
                        _connectedClients.Remove(webSocket);
                        await TryCloseSocket(webSocket);
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
            var buffer = new byte[1024 * 4]; // 4KB buffer
            var messageStream = new MemoryStream();

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await TryCloseSocket(webSocket);
                        break;
                    }

                    // Handle message chunks
                    messageStream.Write(buffer, 0, result.Count);
                    
                    if (result.EndOfMessage)
                    {
                        messageStream.Position = 0;
                        await ProcessMessage(messageStream, webSocket);
                        messageStream.SetLength(0); // Reset for next message
                    }
                }
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                // Client disconnected unexpectedly
                Console.WriteLine("Client disconnected unexpectedly");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                messageStream.Dispose();
            }
        }

        private async Task ProcessMessage(MemoryStream messageStream, WebSocket senderSocket)
        {
            try
            {
                var jsonPayload = Encoding.UTF8.GetString(messageStream.ToArray());
                var data = JsonSerializer.Deserialize<LivestreamModal>(jsonPayload, _jsonOptions);

                if (data != null)
                {
                    Console.WriteLine($"Received data from {data.UserId}: {data.ActiveApp}");

                    // Process and broadcast the message
                    await BroadcastMessage(data);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON error: {ex.Message}");
                await SendError(senderSocket, "Invalid message format");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Processing error: {ex.Message}");
            }
        }

        private async Task BroadcastMessage(LivestreamModal data)
        {
            try
            {
                var response = new
                {
                    data.UserId,
                    data.OrganizationId,
                    data.ActiveApp,
                    data.ActiveUrl,
                    data.LiveStreamStatus,
                    data.ActiveAppLogo,
                    data.ActiveScreenshot,
                    data.Latitude,
                    data.Longitude,
                    Timestamp = DateTime.UtcNow
                };

                var jsonResponse = JsonSerializer.Serialize(response, _jsonOptions);
                var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                foreach (var socket in _connectedClients.ToList())
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        try
                        {
                            await socket.SendAsync(
                                new ArraySegment<byte>(responseBytes),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to send to client: {ex.Message}");
                            _connectedClients.Remove(socket);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Broadcast error: {ex.Message}");
            }
        }

        private async Task SendError(WebSocket socket, string errorMessage)
        {
            try
            {
                var errorResponse = new { Error = errorMessage };
                var jsonResponse = JsonSerializer.Serialize(errorResponse, _jsonOptions);
                var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                await socket.SendAsync(
                    new ArraySegment<byte>(responseBytes),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            catch
            {
                // Ignore errors when sending error messages
            }
        }

        private async Task TryCloseSocket(WebSocket socket)
        {
            try
            {
                if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        CancellationToken.None);
                }
            }
            catch
            {
                // Ignore close errors
            }
        }
    }
}