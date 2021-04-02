
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModularityNET.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Apiks.Module.Auth.Controllers
{
    public class AuthController : Controller
    {
        [Route("signin")]
        public IActionResult Signin()
        {
            return View();
        }
    }
}
