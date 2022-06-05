using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel.User;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Auth.Controller
{
    [Area("Auth")]
    public class Account : NikController
    {
        private readonly UserManager<DataModel.User> _userManager;
        private readonly SignInManager<DataModel.User> _signInManager;
        private readonly ISystemBaseService _iSystemBaseServ;

        public Account(
            UserManager<DataModel.User> userManager, 
            SignInManager<DataModel.User> signInManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _iSystemBaseServ = iSystemBaseServ;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                var theUser = await _userManager.GetUserAsync(HttpContext.User);
                if (User.IsInRole("Admin"))
                {
                    return Redirect("/Panel");
                }
                else
                {
                    return Redirect("/Dashboard");
                }

            }

            ViewBag.Messages = Messages;
            UserRegisterRequest model = new UserRegisterRequest();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("NikAdmin") || User.IsInRole("Admin"))
                {
                    return Redirect("/Panel");
                }
                else
                {
                    return Redirect("/Dashboard");
                }

            }

            if (string.IsNullOrEmpty(request.Firstname))
            {
                AddError("نام را وارد نمایید");
            }

            if (string.IsNullOrEmpty(request.Lastname))
            {
                AddError("نام خانوادگی را وارد نمایید");
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                AddError("آدرس ایمیل را وارد نمایید");
            }

            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                AddError("شماره موبایل را وارد نمایید");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                AddError("رمز عبور را وراد نمایید");
            }

            if (string.IsNullOrEmpty(request.ConfirmPassword))
            {
                AddError("تکرار رمز عبور را وراد نمایید");
            }

            if (request.Password != request.ConfirmPassword)
            {
                AddError("رمز عبور و تکرار آن یکسان نمی باشد");
            }

            ViewBag.Messages = Messages;

            if (Messages.Count(x => x.Type == ViewModel.MessageType.Error) > 0)
            {
                return View(request);
            }

            request.PhoneNumber = request.PhoneNumber.PersianToEnglish();

            if (ModelState.IsValid)
            {
                var userCheck = await _userManager.FindByNameAsync(request.PhoneNumber);
                if (userCheck == null)
                {
                    var user = new DataModel.User
                    {
                        UserName = request.PhoneNumber,
                        NormalizedUserName = request.PhoneNumber,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };
                    var result = await _userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        var profile = new UserProfile
                        {
                            Firstname = request.Firstname,
                            Lastname = request.Lastname,
                            UserId = user.Id
                        };
                        _iSystemBaseServ.iUserProfileServ.Add(profile);
                        await _iSystemBaseServ.iUserProfileServ.SaveChangesAsync();
                        return Redirect("/Auth/Account/Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                AddError("message" + error.Description, "en");
                                //ModelState.AddModelError("message", error.Description);
                            }
                        }
                        ViewBag.Messages = Messages;
                        return View(request);
                    }
                }
                else
                {
                    AddError("این کاربری در سامانه موجود است", "fa");

                    ViewBag.Messages = Messages;
                    //ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("NikAdmin") || User.IsInRole("Admin"))
                {
                    return Redirect("/Panel");
                }
                else
                {
                    return Redirect("/Dashboard");
                }
            }

            LoginRequest model = new LoginRequest();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("NikAdmin") || User.IsInRole("Admin"))
                {
                    return Redirect("/Panel");
                }
                else
                {
                    return Redirect("/Dashboard");
                }
            }

            if (string.IsNullOrEmpty(model.Username))
            {
                AddError("نام کاربری را وارد نمایید");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                AddError("رمز عبور راورد نمایید");
            }

            if (Messages.Count > 0)
            {
                ViewBag.Messages = Messages;
                return View(model);
            }

            model.Username = model.Username.PersianToEnglish();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                if (user != null && !user.EmailConfirmed)
                {
                    AddError("این کاربری مجاز به ورود نمی باشد.");

                    ViewBag.Messages = Messages;
                    return View(model);

                }

                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.Username);
                }

                if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    AddError("رمز عبور یا نام کاربری اشتباه است");

                    ViewBag.Messages = Messages;
                    return View(model);

                }


                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    var IsNik = await _userManager.IsInRoleAsync(user, "NikAdmin");
                    var IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    if (IsNik || IsAdmin)
                    {
                        return Redirect("/Panel");
                    }
                    else
                    {
                        return Redirect("/Dashboard");
                    }
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    AddError("امکان ورود این کاربری ممکن نمی باشد.", "fa");
                    ViewBag.Messages = Messages;
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Auth/Account/Login");
        }

    }
}
