using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Widgets
{
    public class WMenus : ViewComponent
    {
        //private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;
        public List<NikMessage> messages;

        public WMenus(
            IConfiguration config,
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            _iSystemBaseServ = iSystemBaseServ;
            messages = new List<NikMessage>();
        }

        public async Task<IViewComponentResult> InvokeAsync(string key, int size = 8)
        {
            var theCategory = await _iSystemBaseServ.iMenuCategoryServ.FindAsync(x => x.KeyValue == key);
            ViewBag.Contents = theCategory;
            return View("Menu");
        }

    }
}
