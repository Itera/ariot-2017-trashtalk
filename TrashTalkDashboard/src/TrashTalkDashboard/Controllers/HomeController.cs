using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrashTalkDashboard.Data.Repositories;
using TrashTalkDashboard.Models;

namespace TrashTalkDashboard.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class HomeController : Controller
    {
        private readonly IDocumentDbRepository _documentDbRepository;

        public HomeController(IDocumentDbRepository documentDbRepository)
        {
            _documentDbRepository = documentDbRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchString)
        {
            var result =  await _documentDbRepository.GetItemAsync(searchString);
            result.TrashCanStatuses.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));

            return View(result);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
