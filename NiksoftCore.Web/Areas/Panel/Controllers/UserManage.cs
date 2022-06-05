using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.DataModel;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Roles = "NikAdmin")]
    public class UserManage : NikController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;

        public UserManage(
            IConfiguration config,
            SignInManager<User> signInManager, 
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _iSystemBaseServ = iSystemBaseServ;
        }

        public IActionResult Index(UserGridRequest request)
        {
            var query = _iSystemBaseServ.iNikUserServ.ExpressionMaker();
            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query.Add(x => x.UserName.Contains(request.UserName));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.FirstName))
            {
                var userIds = _iSystemBaseServ.iUserProfileServ.GetAll(x => x.Firstname.Contains(request.FirstName)).Select(x => x.UserId);
                query.Add(x => userIds.Contains(x.Id));
                isSearch = true;
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                var userIds = _iSystemBaseServ.iUserProfileServ.GetAll(x => x.Lastname.Contains(request.LastName)).Select(x => x.UserId);
                query.Add(x => userIds.Contains(x.Id));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = _iSystemBaseServ.iNikUserServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;


            ViewBag.PageTitle = "مدیریت دسته بندی ها";

            ViewBag.Contents = _iSystemBaseServ.iNikUserServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.Id, true).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {
            ViewBag.PageTitle = "ایجاد دسته بندی";

            var request = new UserRequest();

            if (Id > 0)
            {
                var item = _iSystemBaseServ.iNikUserServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.UserName = item.UserName;
                request.Email = item.Email;
                request.PhoneNumber = item.PhoneNumber;
            }

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!ValidUserForm(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            User item = new User();
            if (request.Id > 0)
            {
                item = await _userManager.FindByIdAsync(request.Id.ToString());
            }


            item.UserName = request.UserName;
            item.Email = request.Email;
            item.PhoneNumber = request.PhoneNumber;


            if (request.Id == 0)
            {
                item.EmailConfirmed = true;
                item.PhoneNumberConfirmed = true;
                await _userManager.CreateAsync(item, request.Password);
            }
            else
            {
                await _userManager.UpdateAsync(item);
            }


            return Redirect("/Panel/UserManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            await _userManager.DeleteAsync(user);
            return Redirect("/Panel/UserManage");
        }

        public bool ValidUserForm(UserRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.UserName))
            {
                AddError("نام کاربری باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                AddError("آدرس ایمیل باید مقدار داشته باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                AddError("رمز عبور باید مقدار داشته باشد", "fa");
                result = false;
            }
            else if (request.Password.Length < 6)
            {
                AddError("رمز عبور باید بیشتر از 6 کاراکتر باشد", "fa");
                result = false;
            }

            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                AddError("شماره موبایل باید مقدار داشته باشد", "fa");
                result = false;
            }



            return result;
        }

        [HttpGet]
        public async Task<IActionResult> ShowUserRoles(UserRoleRequest request)
        {
            var theUser = await _userManager.FindByIdAsync(request.UserId.ToString());

            var rolNames = await _userManager.GetRolesAsync(theUser);

            var userRoles = _iSystemBaseServ.iNikRoleServ.GetAll(x => rolNames.Contains(x.Name)).ToList();

            ViewBag.PageTitle = "مدیریت نقش ها " + theUser.UserName;

            var allRoles = _roleManager.Roles.Where(x => true).Select(x => new { x.Id, x.Name }).ToList();

            ViewBag.User = theUser;
            ViewBag.Roles = new SelectList(allRoles, "Id", "Name", request?.RoleId);
            ViewBag.Contents = userRoles.ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(UserRoleRequest request)
        {
            var theUser = await _userManager.FindByIdAsync(request.UserId.ToString());
            var theRole = await _iSystemBaseServ.iNikRoleServ.FindAsync(x => x.Id == request.RoleId);
            await _userManager.AddToRoleAsync(theUser, theRole.Name);
            return Redirect("/Panel/UserManage/ShowUserRoles?UserId=" + request.UserId);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveRole(UserRoleRequest request)
        {
            var theUser = await _userManager.FindByIdAsync(request.UserId.ToString());
            var theRole = await _iSystemBaseServ.iNikRoleServ.FindAsync(x => x.Id == request.RoleId);
            await _userManager.RemoveFromRoleAsync(theUser, theRole.Name);
            return Redirect("/Panel/UserManage/ShowUserRoles?UserId=" + request.UserId);
        }


        [HttpGet]
        public async Task<IActionResult> LoginWithUser(int UserId)
        {
            var theUser = await _userManager.FindByIdAsync(UserId.ToString());
            await _signInManager.SignInAsync(theUser, true);
            return Redirect("/Panel");
        }

    }
}
