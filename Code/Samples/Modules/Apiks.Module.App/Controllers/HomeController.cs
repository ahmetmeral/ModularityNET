using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Apiks.Module.App.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult App()
        {
            return View();
        }
    }
}
