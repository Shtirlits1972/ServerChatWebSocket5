using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;

namespace ServerChatWebSocket5.SocketManagers
{
    public class ConnectionManager
    {
          private  ConcurrentDictionary<string, WebSocket> _connections = new ConcurrentDictionary<string, WebSocket>();

          public WebSocket GetSocketById(string id)
          {
              return _connections.FirstOrDefault(x => x.Key == id).Value;
          }

          public ConcurrentDictionary<string, WebSocket> GetAllWebSockets()
          {
              return _connections;
          }

          public string GetId(WebSocket socket)
          {
              return _connections.FirstOrDefault(x => x.Value == socket).Key;
          }

          public async Task RemoveSocketAsync(string id)
          {
              _connections.TryRemove(id, out var socket);
              await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "socket connection closed", CancellationToken.None);
          }

          private string GetConnectionId()
          {
              return Guid.NewGuid().ToString("N");
          }

          public void AddSocket(WebSocket socket)
          {
              _connections.TryAdd(GetConnectionId(), socket);
          }
    }
}
