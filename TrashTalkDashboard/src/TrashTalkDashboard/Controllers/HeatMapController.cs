using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrashTalkDashboard.Data.Repositories;
using TrashTalkDashboard.Models;
using TrashTalkDashboard.Models.HeatMapViewModels;

namespace TrashTalkDashboard.Controllers
{
    [Route("api/heatmap")]
    public class HeatMapController : Controller
    {
        private readonly IDocumentDbRepository _documentDbRepository;

        public HeatMapController(IDocumentDbRepository documentDbRepository)
        {
            _documentDbRepository = documentDbRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("points")]
        public async Task<List<HeatMapPoint>> GetMapPoints()
        {
            var allTrashCans = await _documentDbRepository.GetAllItemsAsync();
            var heatMapPoints = new List<HeatMapPoint>();
            foreach (var trashCan in allTrashCans)
            {
                if (trashCan.LatestReading == null)
                    continue;
                heatMapPoints.Add(MapToMapPoint(trashCan));
            }
            return heatMapPoints;
        }

        private static HeatMapPoint MapToMapPoint(TrashCan trashCan)
        {
            return new HeatMapPoint
            {
                FillGrade = trashCan.LatestReading.FillGrade.GetValueOrDefault(),
                Long = trashCan.Long,
                Lat = trashCan.Lat
            };
        }
    }
}