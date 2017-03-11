using System.Linq;
using System.Threading.Tasks;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using TrashTalkApi.Models;
using TrashTalkApi.Repositories;

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

        public static void SendTrashCanStaus(string deviceId)
        {
            var clients = Clients.Where(c => ((TrashWebSocketHandler)c).DeviceId == deviceId);
            foreach (var c in clients)
            {
                var data = Task.Run(() => DocumentDbRepository<TrashCan>.GetItemAsync(deviceId)).Result;

                if (data != null)
                    ((TrashWebSocketHandler)c).Send(JsonConvert.SerializeObject(data.LatestReading));
            }
        }

        public override void OnClose()
        {
            Clients.Remove(this);
        }
    }
}