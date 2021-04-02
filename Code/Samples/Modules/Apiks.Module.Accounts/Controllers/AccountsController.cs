using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Apiks.Module.Accounts.Controllers
{
    public class AccountsController : Controller
    {
       
        [Route("")]
        public IActionResult Accounts()
        {
            return View();
        }
    }
}
