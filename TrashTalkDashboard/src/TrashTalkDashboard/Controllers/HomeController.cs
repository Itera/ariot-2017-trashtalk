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
            var guid = Guid.Parse("b9686b80-e80c-423b-9c20-d965f3f1bc35");
            //var string1 = "987e7f35-2315-432e-b9ea-77ffb8db80a0";
            //var string2 = "c7883bb1-560d-4d82-b07f-a30aab71f056";
            //var string3 = "a1ee5c57-f02f-4f24-a898-b6610cbe21a7";

            //await _documentDbRepository.DeleteItemAsync(string1);
            //await _documentDbRepository.DeleteItemAsync(string2);
            //await _documentDbRepository.DeleteItemAsync(string3);
            var result = await
                _documentDbRepository.GetItemsAsync(list => list.DeviceId == guid);
            return View(result);
        }

        public IActionResult Delete()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
