using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrashTalkDashboard.Models;

namespace TrashTalkDashboard.Controllers
{
    public class DeviceController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AmountBought,AmountRequested,Description,Url,SortOrder")] TrashCan trashCan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gift);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(gift);
        }
    }
}