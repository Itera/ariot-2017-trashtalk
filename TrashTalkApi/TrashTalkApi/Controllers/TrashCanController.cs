using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using TrashTalkApi.Models;
using TrashTalkApi.Repositories;
using TrashTalkApi.Calculations;

namespace TrashTalkApi.Controllers
{
    [RoutePrefix("api/trashcan")]
    public class TrashCanController : ApiController
    {
        [HttpPost]
        [Route("{deviceId}/status")]
        public async Task<IHttpActionResult> Post([FromBody]StoredTrashCanStatus trashCanStatus, string deviceId)
        {
            var existing = await DocumentDbRepository<TrashCan>.GetItemAsync(deviceId);
            if (existing != null)
            {
                existing.LatestReading = trashCanStatus;
                existing.TrashCanStatuses.Add(trashCanStatus);
                await DocumentDbRepository<TrashCan>.UpdateItemAsync(existing.id, existing);
                return Ok();
            }
            trashCanStatus.Timestamp = DateTime.UtcNow;

            var trashCan = new TrashCan
            {
                id = deviceId,
                TrashCanStatuses = new List<StoredTrashCanStatus> {trashCanStatus},
                LatestReading = trashCanStatus
            };

            await DocumentDbRepository<TrashCan>.CreateItemAsync(trashCan);
            return Ok();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Post()
        {
            var deviceId = Guid.NewGuid();
            var device = new Device { Id = deviceId, ActivationDate = DateTime.UtcNow };
            await DocumentDbRepository<Device>.CreateItemAsync(device);
            return Ok(deviceId);
        }
    }
}
