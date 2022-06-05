using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.API
{
    [Route("/api/[controller]/[action]")]
    public class SystemApi : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSysBaseServ;

        public SystemApi(
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

        [HttpGet]
        public IActionResult GetCountries(int psize, int part, string title)
        {
            var query = _iSysBaseServ.iProvinceServ.QueryMaker(y => y.Where(x => true));

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(x => x.Title.Contains(title));
            }

            var total = query.Count();
            var pager = new Pagination(total, psize, part);

            var items = query.OrderByDescending(x => x.Id).Select(x => new
            {
                x.Id,
                x.Title,
                x.CountryId
            }).Skip(pager.StartIndex).Take(pager.PageSize).ToList();

            return Ok(new
            {
                status = 200,
                message = "کشورها",
                total = total,
                data = items
            });
        }

        [HttpGet]
        public IActionResult GetProvinces(int countryId, int psize, int part, string title)
        {
            var query = _iSysBaseServ.iProvinceServ.QueryMaker(y => y.Where(x => x.CountryId == countryId));

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(x => x.Title.Contains(title));
            }

            var total = query.Count();
            var pager = new Pagination(total, psize, part);

            var items = query.OrderByDescending(x => x.Id).Select(x => new
            {
                x.Id,
                x.Title,
                x.CountryId
            }).Skip(pager.StartIndex).Take(pager.PageSize).ToList();

            return Ok(new
            {
                status = 200,
                message = "استان ها",
                total = total,
                data = items
            });
        }

        [HttpGet]
        public IActionResult GetCities(int provinceId, int psize, int part, string title)
        {
            var query = _iSysBaseServ.iCityServ.QueryMaker(y => y.Where(x => x.ProvinceId == provinceId));

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(x => x.Title.Contains(title));
            }

            var total = query.Count();
            var pager = new Pagination(total, psize, part);

            var items = query.OrderByDescending(x => x.Id).Select(x => new
            {
                x.Id,
                x.Title,
                x.CountryId,
                x.ProvinceId
            }).Skip(pager.StartIndex).Take(pager.PageSize).ToList();

            return Ok(new
            {
                status = 200,
                message = "شهر ها",
                total = total,
                data = items
            });
        }





    }
}
