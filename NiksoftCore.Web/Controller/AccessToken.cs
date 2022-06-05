using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NiksoftCore.MiddlController.Middles;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Controller
{
    [Route("/api/auth/[controller]")]
    public class AccessToken : NikApi
    {
        public IConfiguration Configuration { get; }

        private readonly UserManager<DataModel.User> UserManager;

        public AccessToken(IConfiguration configuration, UserManager<DataModel.User> userManager)
        {
            Configuration = configuration;
            UserManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken([FromForm] string usename, [FromForm] string password)
        {
            DataModel.User user = await UserManager.FindByNameAsync(usename);

            if (user == null)
            {
                return Ok(new { status = 403, message = "access denaid" });
            }

            var isTrust = await UserManager.CheckPasswordAsync(user, password);

            if (!isTrust)
            {
                return Ok(new { status = 403, message = "access denaid" });
            }

            int lifeDaies = Convert.ToInt32(Configuration["TokenOptions:LifeDaies"]);
            DateTime nowTime = DateTime.Now;
            DateTime nowUtcTime = DateTime.UtcNow;
            // Creates the signed JWT
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenOptions:Key"]));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("id", user.Id.ToString())
                }),
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
                create = nowTime.ToString(),
                expair = nowTime.AddDays(lifeDaies).ToString(),
                token = accessToken,
                type = "bearer"
            });
        }
    }
}
