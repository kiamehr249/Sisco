using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.DataModel;

namespace NiksoftCore.Web.Controller
{
    public class PageContent : NikController
    {
        private readonly UserManager<DataModel.User> _userManager;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly ISystemBaseService _iSystemBaseServ;

        public PageContent(
            IWebHostEnvironment hostingEnv,
            UserManager<DataModel.User> userManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _userManager = userManager;
            _hostingEnv = hostingEnv;
            _iSystemBaseServ = iSystemBaseServ;
        }

        public IActionResult Index(string Id)
        {

            int itemId = 0;
            GeneralContent pageItem;
            if (int.TryParse(Id, out itemId))
            {
                pageItem = _iSystemBaseServ.iGeneralContentServ.Find(x => x.Id == itemId);
            }
            else
            {
                pageItem = _iSystemBaseServ.iGeneralContentServ.Find(x => x.KeyValue == Id);
            }


            ViewBag.Content = pageItem;
            ViewData["Title"] = pageItem.Title;

            return View();
        }


    }
}
