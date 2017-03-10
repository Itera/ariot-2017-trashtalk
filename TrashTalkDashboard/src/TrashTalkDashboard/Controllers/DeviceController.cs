using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrashTalkDashboard.Data.Repositories;
using TrashTalkDashboard.Models;

namespace TrashTalkDashboard.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IDocumentDbRepository _documentDbRepository;

        public DeviceController(IDocumentDbRepository documentDbRepository)
        {
            _documentDbRepository = documentDbRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(decimal longitude, decimal latitude)
        {
            var trashCanId = Guid.NewGuid();
            var trashCan = new TrashCan
            {
                id = trashCanId.ToString(),
                TrashCanStatuses = new List<TrashCanStatus>()
            };
            await _documentDbRepository.CreateItemAsync(trashCan);
            return Ok(trashCanId);
        }
    }
}