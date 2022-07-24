using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerChatWebSocket5.SocketManagers
{
    public class SocketMiddleware
    {
        private readonly RequestDelegate _next;
        private SocketHandler Handler { get; set; }

        public SocketMiddleware(RequestDelegate next, SocketHandler handler)
        {
            _next = next;
            Handler = handler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await Handler.OnConnected(socket);

            await Recieve(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await Handler.Recieve(socket, result, buffer);
                }
                else if(result.MessageType == WebSocketMessageType.Close)
                {
                    await Handler.OnDisconnected(socket);
                }
            });
        }

        private async Task Recieve(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> messageHandler)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                messageHandler(result,buffer);
            }
        }
    }
}