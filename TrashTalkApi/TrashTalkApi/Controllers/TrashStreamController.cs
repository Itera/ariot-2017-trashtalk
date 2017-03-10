using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.Web.WebSockets;
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
            HttpContext.Current.AcceptWebSocketRequest(new TrashWebSocketHandler());
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }
    }
}
