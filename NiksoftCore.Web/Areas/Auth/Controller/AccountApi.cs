using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Auth.Controller
{
    [Route("/api/base/[controller]/[action]")]
    public class AccountApi : NikApi
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;

        public AccountApi(
            IConfiguration config, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _iSystemBaseServ = iSystemBaseServ;
        }

        [HttpPost]
        public async Task<IActionResult> SignInUser([FromForm] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در مقادیر ورودی"
                });
            }

            request.Username = request.Username.PersianToEnglish();

            var user = await _userManager.FindByEmailAsync(request.Username);
            if (user != null && !user.EmailConfirmed)
            {
                return Ok(new
                {
                    status = 402,
                    message = "این نام کابری هنوز تایید نشده است"
                });

            }

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(request.Username);
            }

            if (await _userManager.CheckPasswordAsync(user, request.Password) == false)
            {

                return Ok(new
                {
                    status = 402,
                    message = "این کاربری نا معتبر است"
                });
            }

            var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, request.RememberMe, true);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    status = 200,
                    message = "ورود موفق",
                    data = true
                });
            }
            else if (result.IsLockedOut)
            {
                return Ok(new
                {
                    status = 403,
                    message = "عدم دسترسی به سامانه",
                    data = false
                });
            }
            else
            {
                return Ok(new
                {
                    status = 403,
                    message = "عدم دسترسی به سامانه",
                    data = false
                });
            }
        }


        [HttpPost]
        public IActionResult SearchProfile(string title)
        {

            if (string.IsNullOrEmpty(title) || title.Length < 4)
            {
                return Ok(new
                {
                    status = 404,
                    message = "خطا در مقادیر ورودی",
                    data = new List<string>()
                });
            }

            var theProfiles = _iSystemBaseServ.iUserProfileServ.GetAll(x => x.CompanyName.Contains(title), y => new { 
                y.Id,
                Title = y.CompanyName
            }, 0, 10).ToList();

            return Ok(new
            {
                status = 200,
                message = "دریافت اطلاعات",
                data = theProfiles
            });
        }
    }
}
