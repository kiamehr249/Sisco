using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using System.Linq;

namespace NiksoftCore.Web.Controller
{
    [Microsoft.AspNetCore.Mvc.Route("/api/base/[controller]/[action]")]
    public class AddressApi : NikApi
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;

        public AddressApi(
            IConfiguration config, 
            UserManager<User> userManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            _userManager = userManager;
            _iSystemBaseServ = iSystemBaseServ;
        }

        [HttpPost]
        public IActionResult GetCity([FromForm] int provinceId)
        {
            var query = _iSystemBaseServ.iCityServ.ExpressionMaker();

            if (provinceId != 0)
            {
                query.Add(x => x.ProvinceId == provinceId);
            }

            var cities = _iSystemBaseServ.iCityServ.GetAll(query, y => new {
                y.Id,
                y.Title,
                y.CountryId,
                y.ProvinceId
            }).ToList();

            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                count = cities.Count,
                data = cities
            });
        }

        [HttpPost]
        //[Authorize(Policy = "AccessToken")]
        public IActionResult GetProvince([FromForm] int countryId)
        {
            if (countryId == 0)
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در مقادیر ورودی"
                });
            }

            var provinces = _iSystemBaseServ.iProvinceServ.GetAll(x => x.CountryId == countryId, y => new {
                y.Id,
                y.Title,
                y.CountryId
            }).ToList();

            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                count = provinces.Count,
                data = provinces
            });
        }


    }
}