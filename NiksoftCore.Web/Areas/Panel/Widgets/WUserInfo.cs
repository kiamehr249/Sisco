using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Panel.Widgets
{
    public class WUserInfo : ViewComponent
    {
        private readonly UserManager<DataModel.User> _userManager;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;
        public List<NikMessage> messages;

        public WUserInfo(
            IConfiguration config, 
            UserManager<DataModel.User> userManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            messages = new List<NikMessage>();
            _iSystemBaseServ = iSystemBaseServ;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var thisUser = await _userManager.GetUserAsync(HttpContext.User);
                var thisProfile = await _iSystemBaseServ.iUserProfileServ.FindAsync(x => x.UserId == thisUser.Id);

                ViewBag.Profile = thisProfile;
                ViewBag.User = thisUser;
            }

            return View();
        }

    }
}
