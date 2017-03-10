﻿using System.Linq;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using TrashTalkApi.Models;

namespace TrashTalkApi.WebSocket
{
    public class TrashWebSocketHandler : WebSocketHandler
    {
        private static readonly WebSocketCollection Clients = new WebSocketCollection();
        public string DeviceId { get; set; }

        public override void OnOpen()
        {
            DeviceId = WebSocketContext.QueryString["deviceId"];
            Clients.Add(this);
        }

        public void SendMessage(string deviceId, TrashCanStatus message)
        {
            var client = Clients.FirstOrDefault(c => DeviceId == deviceId);
            client?.Send(JsonConvert.SerializeObject(message));
        }

        public override void OnClose()
        {
            Clients.Remove(this);
        }
    }
}