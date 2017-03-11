using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrashTalkDashboard.Data.Repositories;
using TrashTalkDashboard.Models;

namespace TrashTalkDashboard.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IDocumentDbRepository _documentDbRepository;

        public StatisticsController(IDocumentDbRepository documentDbRepository)
        {
            _documentDbRepository = documentDbRepository;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _documentDbRepository.GetAllItemsAsync();
            var trashCans = new List<TrashCan>();
            var viewModel = new StatisticsViewModel();
            foreach (var trashCan in result)
            {
                if (trashCan.LatestReading != null)
                    trashCans.Add(trashCan);
            }
            viewModel.TrashCans = trashCans;
            viewModel.AverageFillPercentage = Math.Round(trashCans.Average(x => x.LatestReading.FillGrade.GetValueOrDefault()) * 100);
            viewModel.AverageWeight = Math.Round(trashCans.Average(x => x.LatestReading.Weight));
            
            viewModel.CircleClassFillPercentage = GetCircleClassFill(viewModel.AverageFillPercentage);
            viewModel.CircleClassWeigth = GetCircleClassWeight(viewModel.AverageWeight);
            return View(viewModel);
        }

        private string GetCircleClassFill(decimal fillPercentageAllTrashCans)
        {
            if (fillPercentageAllTrashCans > 0 && fillPercentageAllTrashCans < 20)
                return "greenCircle";
            if (fillPercentageAllTrashCans > 20 && fillPercentageAllTrashCans < 40)
                return "yellowCircle";
            if (fillPercentageAllTrashCans > 40 && fillPercentageAllTrashCans < 60)
                return "orangeCircle";
            return "redCircle";
        }
        private string GetCircleClassWeight(decimal weight)
        {
            if (weight > 0 && weight < 11)
                return "greenCircle";
            if (weight > 10 && weight < 15)
                return "yellowCircle";
            if (weight > 15 && weight < 30)
                return "orangeCircle";
            return "redCircle";
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
