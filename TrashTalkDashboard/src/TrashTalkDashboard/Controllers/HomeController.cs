using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrashTalkDashboard.Data.Repositories;

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

        public async Task<IActionResult> Index()
        {
            var guid = "b9686b80-e80c-423b-9c20-d965f3f1bc35";
            //await _documentDbRepository.DeleteItemAsync(guid);
            var result = await _documentDbRepository.GetItemsAsync(list => list.id == guid);
            return View(result);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
