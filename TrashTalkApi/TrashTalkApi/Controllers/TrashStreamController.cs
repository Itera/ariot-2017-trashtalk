using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.Web.WebSockets;
using TrashTalkApi.Models;
using TrashTalkApi.WebSocket;


namespace TrashTalkApi.Controllers
{
    [RoutePrefix("api/trashstream")]
    public class TrashStreamController : ApiController
    {
        [HttpGet]
        [Route("{deviceId}")]
        public HttpResponseMessage Get(string deviceId)
        {
            HttpContext.Current.AcceptWebSocketRequest(new TrashWebSocketHandler(deviceId));
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }

        [HttpGet]
        [Route("{deviceId}/test")]
        public HttpResponseMessage GetTest(string deviceId)
        {
            TrashWebSocketHandler.SendMessage(deviceId, new TrashCanStatus() {LidIsClosed = false});
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
