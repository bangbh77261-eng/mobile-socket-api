using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

    public class WebSocketManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _connections = new();

        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var connectionId = Guid.NewGuid().ToString();
            _connections.TryAdd(connectionId, webSocket);

            Console.WriteLine($"Client {connectionId} connected");

            // Send welcome message
            await SendMessageAsync(webSocket, new
            {
                type = "welcome",
                message = "Connected to WebSocket server",
                connectionId = connectionId
            });

            var buffer = new byte[1024 * 4];

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None
                    );

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await HandleMessage(connectionId, message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _connections.TryRemove(connectionId, out _);
                Console.WriteLine($"Client {connectionId} disconnected");
            }
        }

        private async Task HandleMessage(string connectionId, string message)
        {
            try
            {
                var data = JsonSerializer.Deserialize<MessageModel>(message);
                Console.WriteLine($"Received from {connectionId}: {message}");

                // Echo back to sender
                if (_connections.TryGetValue(connectionId, out var senderSocket))
                {
                    await SendMessageAsync(senderSocket, new
                    {
                        type = "echo",
                        data = data,
                        timestamp = DateTime.UtcNow
                    });
                }

                // Broadcast to all clients
                await BroadcastAsync(new
                {
                    type = "broadcast",
                    from = connectionId,
                    data = data,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling message: {ex.Message}");
            }
        }

        private async Task SendMessageAsync(WebSocket webSocket, object message)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                var json = JsonSerializer.Serialize(message);
                var buffer = Encoding.UTF8.GetBytes(json);
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            }
        }

        public async Task BroadcastAsync(object message)
        {
            var tasks = new List<Task>();

            foreach (var connection in _connections.Values)
            {
                if (connection.State == WebSocketState.Open)
                {
                    tasks.Add(SendMessageAsync(connection, message));
                }
            }

            await Task.WhenAll(tasks);
        }
    }

    public class MessageModel
    {
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
    }
