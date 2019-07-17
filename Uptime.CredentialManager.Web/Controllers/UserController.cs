using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Uptime.CredentialManager.Web.Models;

namespace Uptime.CredentialManager.Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            var users = new List<User>
            {
                new User{ Id= Guid.NewGuid(), Name= "Saskia" }
            };
            return View(users);
        }
    }
}