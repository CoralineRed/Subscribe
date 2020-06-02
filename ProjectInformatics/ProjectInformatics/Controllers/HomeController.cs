using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectInformatics.Entities;
using ProjectInformatics.Models;
using ProjectInformatics.Services;
using ProjectInformatics.Database;
using Microsoft.Extensions.Logging;
using ProjectInformatics.Logging;
using System;

namespace ProjectInformatics.Controllers
{
    public class HomeController : Controller
    {
        private IDbService db;

        public HomeController(IDbService context, ILogger<HomeController> logger)
        {
            db = LoggingAdvice<IDbService>.Create(context, logger);
        }

   
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AllUsers()
        {
            var jsondata = db.GetUsers();
            return new JsonResult(jsondata);
        }

        public IActionResult AllMessages()
        {
            var jsondata = db.GetMessages();
            return new JsonResult(jsondata);
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminPage()
        {
            return View();
        }

        [Authorize]
        public IActionResult MyServices(string order)
        {
            ViewBag.CategoryId = db.GetUserCategory(User.Identity.Name);
            return View(db.GetSubscriptions(User.Identity.Name, order));
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddService()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddService(Subscription model)
        {
            if (ModelState.IsValid)
            {
                db.AddServiceToUser(model, User.Identity.Name);
                return RedirectToAction("MyServices", "Home");
            }
            return View(model);
        }

        [Authorize]
        public IActionResult ThankYou(int categoryId)
        {
            db.UpdateUserCategory(User.Identity.Name, categoryId);
            db.AddServiceToUser(new Subscription()
                {
                    Name = "ProjectInformatics",
                    LastPayment = DateTime.Today,
                    Period = 30,
                    Price = 15
                },
                User.Identity.Name);
            return View();
        }
    }
}
