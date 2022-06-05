using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Linq;

namespace NiksoftCore.Web.Controller
{
    public class News : NikController
    {
        private readonly UserManager<DataModel.User> _userManager;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly ISystemBaseService _iSystemBaseServ;
        public News(
            IWebHostEnvironment hostingEnv,
            UserManager<DataModel.User> userManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _userManager = userManager;
            _hostingEnv = hostingEnv;
            _iSystemBaseServ = iSystemBaseServ;
        }

        public IActionResult Index(ContentGridRequest request)
        {
            var query = _iSystemBaseServ.iGeneralContentServ.ExpressionMaker();
            query.Add(x => x.ContentCategory.KeyValue == "news");

            var total = _iSystemBaseServ.iGeneralContentServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = _iSystemBaseServ.iGeneralContentServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            ViewData["Title"] = "News";

            return View();
        }

        public IActionResult Single(int Id)
        {
            var theNews = _iSystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            ViewBag.Content = theNews;
            var sameNews = _iSystemBaseServ.iGeneralContentServ.GetPart(x => x.CategoryId == theNews.CategoryId, 0, 5, x => x.Id, true);
            ViewBag.SameContent = sameNews;


            ViewData["Title"] = theNews.Title;
            ViewBag.PageTitle = theNews.Title;

            return View();
        }


    }
}
