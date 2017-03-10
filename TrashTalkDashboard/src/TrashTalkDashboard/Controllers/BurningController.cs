using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrashTalkDashboard.Data.Repositories;

namespace TrashTalkDashboard.Controllers
{
    public class BurningController : Controller
    {
        private readonly IDocumentDbRepository _documentDbRepository;

        public BurningController(IDocumentDbRepository documentDbRepository)
        {
            _documentDbRepository = documentDbRepository;
        }
        public async Task<IActionResult> Index()
        {
            var burningTrashCans = await _documentDbRepository.GetItemsAsync(x => x.LatestReading != null && x.LatestReading.Flame < 900 && x.LatestReading.LidIsClosed);
            return View(burningTrashCans);
        }
    }
}