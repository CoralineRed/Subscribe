using System;
using System.Collections.Generic;
using System.Linq;
using ProjectInformatics.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectInformatics.Database;

namespace ProjectInformatics.Controllers
{
    public class ChatController : Controller
    {
        private IDbService db;
        public ChatController(IDbService context)
        {
            db = context;
        }
        public IActionResult Support()
        {
            var messages = db.GetMessages();
            messages.Reverse();
            return View(messages);
        }
    }
}