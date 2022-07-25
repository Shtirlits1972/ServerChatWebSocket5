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
                    try
                    {
                        await Handler.Recieve(socket, result, buffer);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if(result.MessageType == WebSocketMessageType.Close)
                {
                    await Handler.OnDisconnected(socket);
                }
            });
        }

        private async Task Recieve(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> messageHandler)
        {
            //  var buffer = new byte[1024 * 4];
            var buffer = new byte[4096 * 8];
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    int a = 0;

                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    messageHandler(result, buffer);


                    int y = 0;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}