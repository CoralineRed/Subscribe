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
using ProjectInformatics.Models;
using ProjectInformatics.Services;

namespace ProjectInformatics.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext _context;
        UserService userService;
        public HomeController(ApplicationContext context, UserService service)
        {
            _context = context;
            userService = service;
        }

        [HttpGet("signin-google", Name = "signin-google")]
        [AllowAnonymous]
        public Task<IActionResult> externallogincallback(string returnUrl = null, string remoteError = null)
        {
            //Here we can retrieve the claims
            var result = HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return null;
        }
        [Authorize]
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
    }
}
