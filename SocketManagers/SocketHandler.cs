using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerChatWebSocket5.SocketManagers
{
    public abstract class SocketHandler
    {
        public  ConnectionManager Connections { get; set; }

        public  SocketHandler(ConnectionManager connections)
        {
            Connections = connections;
        }
        public virtual async Task OnConnected(WebSocket socket)
        {
            await Task.Run(() =>
            {
                Connections.AddSocket(socket);
            });
        }
        public virtual async Task OnDisconnected(WebSocket socket)
        {
              await  Connections.RemoveSocketAsync(Connections.GetId(socket));
        }
        public async Task SendMessage(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
            {
                return;
            }
            int y = 0;


            try
            {
                //   ASCII
                //await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length), WebSocketMessageType.Text, true,
                //    CancellationToken.None);

                await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true,  CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task SendMessage(string id, string message)
        {
            await SendMessage(Connections.GetSocketById(id), message);
        }

        public async Task SendMessageToAll(string message)
        {
            foreach (var con in Connections.GetAllWebSockets())
            {
                await SendMessage(con.Value, message);
            }
        }

        public abstract Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);

    }
}
