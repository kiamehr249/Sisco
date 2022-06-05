﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.ViewModel;
using System.Diagnostics;

namespace NiksoftCore.Web.Controller
{
    public class Home : NikController
    {
        private readonly ILogger<Home> _logger;

        public Home(ILogger<Home> logger, IConfiguration Configuration)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return Redirect("/Auth/Account/Login");
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CustomPage()
        {
            return View("Privacy");
        }

    }
}
