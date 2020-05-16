using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProjectInformatics.Controllers
{
    public class ChatController : Controller
    {
        private ApplicationContext db;
        public ChatController(ApplicationContext context)
        {
            db = context;
        }
        public IActionResult Support()
        {
            return View(db);
        }
    }
}