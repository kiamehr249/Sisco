using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Panel.Widgets
{
    public class WPanelMenu : ViewComponent
    {
        private readonly UserManager<DataModel.User> _userManager;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;
        

        public List<NikMessage> messages;

        public WPanelMenu(
            IConfiguration config, 
            UserManager<DataModel.User> userManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _userManager = userManager;
            _config = config;
            _iSystemBaseServ = iSystemBaseServ;
            messages = new List<NikMessage>();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var thisUser = await _userManager.GetUserAsync(HttpContext.User);
                var userRoles = await _userManager.GetRolesAsync(thisUser);

                ViewBag.Roles = userRoles.ToList();

                var menus = _iSystemBaseServ.iPanelMenuService.GetPart(x => x.Enabled && x.ParentId == null, 0, 50).OrderBy(x => x.Ordering).ToList();
                List<PanelMenu> permits = new List<PanelMenu>();
                foreach (var menu in menus)
                {
                    if (string.IsNullOrEmpty(menu.Roles))
                    {
                        permits.Add(menu);
                    }
                    else
                    {
                        if (menu.Roles.Contains(","))
                        {
                            var readies = menu.Roles.Split(',');
                            //bool oneAdd = false;
                            foreach (var item in readies)
                            {
                                if (userRoles.Contains(item))
                                {
                                    permits.Add(menu);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (userRoles.Contains(menu.Roles))
                            {
                                permits.Add(menu);
                            }
                        }

                    }
                }

                ViewBag.Menus = permits;
            }


            return View();
        }

    }
}
