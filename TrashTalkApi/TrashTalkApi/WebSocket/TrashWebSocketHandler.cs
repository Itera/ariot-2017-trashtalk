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
        
        public string DeviceId { get; set; }

        public TrashWebSocketHandler(string deviceId)
        {
            DeviceId = deviceId;
            Clients.Add(this);
        }

        public static void SendMessage(string deviceId, TrashCanStatus message)
        {
            Clients.FirstOrDefault(c => ((TrashWebSocketHandler) c).DeviceId == deviceId)?
                .Send(JsonConvert.SerializeObject(message));
        }

        public override void OnClose()
        {
            Clients.Remove(this);
        }
    }
}