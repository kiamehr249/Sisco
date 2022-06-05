using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NiksoftCore.MiddlController.Middles;

namespace NiksoftCore.Web.Areas.Panel.Controllers
{
    [Authorize]
    [Area("Panel")]
    public class Home : NikController
    {

        public Home()
        {
        }

        public IActionResult Index()
        {
            ViewBag.PageTitle = "داشبورد";
            return View();
        }
    }
}
