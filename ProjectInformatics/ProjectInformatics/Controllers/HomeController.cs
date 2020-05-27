using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectInformatics.Entities;
using ProjectInformatics.Models;
using ProjectInformatics.Services;
using Microsoft.AspNetCore.Builder;

namespace ProjectInformatics.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        UserService userService;
        public HomeController(ApplicationContext context, UserService service)
        {
            db = context;
            userService = service;

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
            var jsondata = db.Users.ToList<User>();
            return new JsonResult(jsondata);
        }
        public IActionResult AdminPage()
        {
            return View();
        }

        [Authorize]
        public IActionResult MyServices()
        {
            ViewBag.CategoryId = db.GetUserCategory(User.Identity.Name);
            return View(db.GetSubscriptions(User.Identity.Name));
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
            return View();
        }
    }
}
