﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using TrashTalkApi.Models;
using TrashTalkApi.Repositories;

namespace TrashTalkApi.Controllers
{
    [RoutePrefix("api/trashcan")]
    public class TrashCanController : ApiController
    {
        [HttpGet]
        [Route("{deviceId}/status")]
        public async Task<IHttpActionResult> Get(Guid deviceId)
        {
            var device = await DocumentDbRepository<Device>.GetItemAsync(deviceId.ToString());
            if (device == null)
                return BadRequest("Device is not registered");
            var result = await DocumentDbRepository<TrashCanStatus>.GetItemsAsync(list => list.DeviceId == deviceId);
            return Ok(result);
        }

        [HttpPost]
        [Route("{deviceId}/status")]
        public async Task<IHttpActionResult> Post([FromBody]TrashCanStatus trashCanStatus, Guid deviceId)
        {
            trashCanStatus.DeviceId = deviceId;
            trashCanStatus.Timestamp = DateTime.UtcNow;
            if (trashCanStatus.Distance < 0)
                return BadRequest("Negative distance");
            await DocumentDbRepository<TrashCanStatus>.CreateItemAsync(trashCanStatus);
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
