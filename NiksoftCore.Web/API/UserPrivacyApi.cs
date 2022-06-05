using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NiksoftCore.DataModel;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Web.API
{
    [Route("/api/[controller]/[action]")]
    public class UserPrivacyApi : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSysBaseServ;

        public UserPrivacyApi(
            IConfiguration config,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ISystemBaseService iSysBaseServ
            )
        {
            _config = config;
            _userManager = userManager;
            _iSysBaseServ = iSysBaseServ;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);

            if (user == null)
            {
                return StatusCode(400, new { message = "این کاربری یافت نشد", data = new { } });
            }

            var isTrust = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isTrust)
            {
                return StatusCode(401, new { message = "رمز عبور یا نام کاربری اشتباه است", data = new { } });
            }

            var claims = new List<Claim>
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            int lifeDaies = Convert.ToInt32(_config["TokenOptions:LifeDaies"]);
            DateTime nowTime = DateTime.Now;
            DateTime nowUtcTime = DateTime.UtcNow;

            // Creates the signed JWT
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenOptions:Key"]));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = nowUtcTime.AddDays(lifeDaies),
                Issuer = "niksoftgroup.ir",
                Audience = "niksoftgroup.ir",
                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Returns the 'access_token' and the type in lower case
            return Ok(new
            {
                message = "دریافت موفق",
                data = new
                {
                    create = nowTime.ToString(),
                    expire = nowTime.AddDays(lifeDaies).ToString(),
                    token = accessToken,
                    type = "bearer"
                }

            });
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "NikAdmin,Admin")]
        public IActionResult TestRole()
        {
            return Ok("Accessed to this API");
        }

    }
}
