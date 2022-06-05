using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.DataModel;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using NiksoftCore.ViewModel.User;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin")]
    public class RoleManage : NikController
    {
        private readonly UserManager<DataModel.User> _userManager;
        private readonly RoleManager<DataModel.Role> _roleManager;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;

        public RoleManage(
            IConfiguration config,
            UserManager<DataModel.User> userManager,
            RoleManager<DataModel.Role> roleManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _iSystemBaseServ = iSystemBaseServ;
        }

        public IActionResult Index(BaseRequest request)
        {
            ViewBag.PageTitle = "مدیریت نقش ها";

            var query = _iSystemBaseServ.iNikRoleServ.ExpressionMaker();
            query.Add(x => true);
            var total = _iSystemBaseServ.iNikRoleServ.Count(query);
            var pager = new Pagination(total, 20, request.part);
            ViewBag.Pager = pager;

            ViewBag.Roles = _iSystemBaseServ.iNikRoleServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {

            ViewBag.PageTitle = "ایجاد نقش";

            var request = new RoleRequest();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                AddError("نام نقش کاربری باید مقدار داشته باشد", "fa");

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            await _roleManager.CreateAsync(new DataModel.Role
            {
                Name = request.Name
            });

            return Redirect("/Panel/RoleManage");

        }


        [HttpGet]
        public IActionResult Edit(int Id)
        {

            var theRole = _roleManager.Roles.First(x => x.Id == Id);
            var request = new RoleRequest
            {
                Id = theRole.Id,
                Name = theRole.Name,
                NormalizedName = theRole.NormalizedName
            };
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                AddError("نام نقش کاربری باید مقدار داشته باشد", "fa");

            if (Messages.Any(x => x.Type == MessageType.Error))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            var theRole = _roleManager.Roles.First(x => x.Id == request.Id);
            theRole.Name = request.Name;
            await _roleManager.UpdateAsync(theRole);

            return Redirect("/Panel/RoleManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theRole = _roleManager.Roles.First(x => x.Id == Id);
            await _roleManager.DeleteAsync(theRole);
            return Redirect("/Panel/RoleManage");
        }
    }
}
