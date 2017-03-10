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
        [HttpGet]
        [Route("{deviceId}/status/latest")]
        public async Task<IHttpActionResult> Get(string deviceId)
        {
            var trashCan = await DocumentDbRepository<TrashCan>.GetItemAsync(deviceId);
            if (trashCan == null)
                return NotFound();
            return Ok(trashCan.LatestReading);
        }

        [HttpPost]
        [Route("{deviceId}/status")]
        public async Task<IHttpActionResult> Post([FromBody]TrashCanStatus trashCanStatus, string deviceId)
        {
            var storedTrashCanStatus = TrashCanReadingMapper.MapToStoredCanStatus(trashCanStatus);
            var existing = await DocumentDbRepository<TrashCan>.GetItemAsync(deviceId);
            if (existing != null)
            {
                trashCanStatus.Timestamp = DateTime.UtcNow;
                existing.LatestReading = storedTrashCanStatus;
                existing.TrashCanStatuses.Add(storedTrashCanStatus);
                await DocumentDbRepository<TrashCan>.UpdateItemAsync(existing.id, existing);
                return Ok();
            }
            trashCanStatus.Timestamp = DateTime.UtcNow;

            var trashCan = new TrashCan
            {
                id = deviceId,
                TrashCanStatuses = new List<StoredTrashCanStatus> { storedTrashCanStatus },
                LatestReading = storedTrashCanStatus
            };

            await DocumentDbRepository<TrashCan>.CreateItemAsync(trashCan);
            return Ok();
        }

    }
}
