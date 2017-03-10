using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using TrashTalkApi.Models;

namespace TrashTalkApi.WebSocket
{
    public class TrashWebSocketHandler : WebSocketHandler
    {
        private static readonly WebSocketCollection Clients = new WebSocketCollection();
        //private static readonly List<TrashWebSocketHandler> Clients = new List<TrashWebSocketHandler>();
        public string DeviceId { get; set; }

        public override void OnOpen()
        {
            DeviceId = WebSocketContext.QueryString["deviceId"];
            Clients.Add(this);
        }

        public static void SendMessage(string deviceId, TrashCanStatus message)
        {
            Clients.Broadcast("FooBar: " + Clients.Count);
            var client = Clients.FirstOrDefault(c => ((TrashWebSocketHandler)c).DeviceId == deviceId);

            var webSocketCollection = new WebSocketCollection { client };
            webSocketCollection.Broadcast(JsonConvert.SerializeObject(message));
            //client?.Send(JsonConvert.SerializeObject(message));

            //foreach (var c in Clients)
            //{
            //    c.Send(((TrashWebSocketHandler)c).DeviceId);
            //}
        }

        public override void OnClose()
        {
            Clients.Remove(this);
        }
    }
}