using System;
using System.Threading.Tasks;
using System.Web.Http;
using TrashTalkApi.Repositories;

namespace TrashTalkApi.Controllers
{
    [RoutePrefix("api/device")]
    public class DeviceController : ApiController
    {
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Post()
        {
            var deviceId = Guid.NewGuid();
            var device = new Device {Id = deviceId, ActivationDate = DateTime.UtcNow};
            await DocumentDbRepository<Device>.CreateItemAsync(device);
            return Ok(deviceId);
        }
    }

    public class Device 
    {
        public Guid Id { get; set; }
        public DateTime ActivationDate { get; set; }
    }
}
