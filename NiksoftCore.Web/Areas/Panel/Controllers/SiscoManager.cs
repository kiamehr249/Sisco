using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Sisco.Controller
{
    [Area("Panel")]
    [Authorize]
    public class SiscoManager : NikController
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSysBaseServ;

        public SiscoManager(
            IConfiguration config,
            IWebHostEnvironment hostingEnv,
            UserManager<User> userManager,
             ISystemBaseService iSysBaseServ
            )
        {
            _userManager = userManager;
            _hostingEnv = hostingEnv;
            _config = config;
            _iSysBaseServ = iSysBaseServ; ;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var records = new List<SiscoBaseModel>();
            ViewBag.Records = records;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(CsvDataRequest request)
        {
            using var memoryStream = new MemoryStream(new byte[request.Source.Length]);
            await request.Source.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var records = new List<SiscoBaseModel>();

            using (var reader = new StreamReader(memoryStream))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    records = csv.GetRecords<SiscoBaseModel>().ToList();
                }
            }

            //var fileextension = Path.GetExtension(request.Source.FileName);
            //var filename = Guid.NewGuid().ToString() + fileextension;
            //var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files/Sisco", filename);
            //using (FileStream fs = System.IO.File.Create(filepath))
            //{
            //    request.Source.CopyTo(fs);
            //}

            //var records = new List<SiscoBaseModel>();
            //if (fileextension == ".csv")
            //{
            //    using (var reader = new StreamReader(filepath))
            //    {
            //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            //        {
            //            records = csv.GetRecords<SiscoBaseModel>().ToList();
            //        }
            //    }
            //}

            ViewBag.Records = records;

            return View();
        }

    }
}
