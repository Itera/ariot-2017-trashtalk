using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using TrashTalkApi.Models;
using TrashTalkApi.Repositories;
using TrashTalkApi.Calculations;
using TrashTalkApi.WebSocket;

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
        [Route("create")]
        public async Task<IHttpActionResult> Create(TrashCan trashCan)
        {
            var trashCanId = Guid.NewGuid();
            trashCan.id = trashCanId.ToString();
            trashCan.TrashCanStatuses = new List<StoredTrashCanStatus>();
            await DocumentDbRepository<TrashCan>.CreateItemAsync(trashCan);
            return Ok(trashCanId);
        }

        [HttpPost]
        [Route("{deviceId}/status")]
        public async Task<IHttpActionResult> Post([FromBody]TrashCanStatus trashCanStatus, string deviceId)
        {
            var storedTrashCanStatus = TrashCanReadingMapper.MapToStoredCanStatus(trashCanStatus);
            var existing = await DocumentDbRepository<TrashCan>.GetItemAsync(deviceId);
            if (existing == null)
                return NotFound();
            storedTrashCanStatus.Timestamp = DateTime.UtcNow;
            existing.LatestReading = storedTrashCanStatus;
            existing.TrashCanStatuses.Add(storedTrashCanStatus);
            await DocumentDbRepository<TrashCan>.UpdateItemAsync(existing.id, existing);
            
            var trashStream = new TrashWebSocketHandler();
            trashStream.SendMessage(deviceId, trashCanStatus);

            return Ok();
        }

    }
}
