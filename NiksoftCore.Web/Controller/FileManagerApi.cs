using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.DataModel;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Controller
{
    [Microsoft.AspNetCore.Mvc.Route("/api/base/[controller]/[action]")]
    public class FileManagerApi : NikApi
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;

        public FileManagerApi(
            IConfiguration config, 
            UserManager<User> userManager, 
            IWebHostEnvironment hostingEnv,
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            _userManager = userManager;
            _iSystemBaseServ = iSystemBaseServ;
            _hostingEnv = hostingEnv;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ContentId))
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در ورودی، شناسه محتوایی ارسال نشده است"
                });
            }

            if (string.IsNullOrWhiteSpace(request.ContentKey))
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در ورودی، کلید محتوایی ارسال نشده است"
                });
            }

            if (string.IsNullOrWhiteSpace(request.RootKey))
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در ورودی، کلید ریشه ارسال نشده است"
                });
            }

            if (request.File == null)
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در ورودی، هیچ فایلی جهت آپلود ارسال نشده است"
                });
            }

            string fileRoot = _config.GetSection("FileRoot:" + request.RootKey).Value;

            if (string.IsNullOrWhiteSpace(fileRoot))
            {
                return Ok(new
                {
                    status = 401,
                    message = "مجاز به انجام عملیات نیستید"
                });
            }

            List<string> uploads = new List<string>();

            if (request.File.Length > 0)
            {
                var fileName = Path.GetFileName(request.File.FileName);
                var folderPath = Path.Combine(_hostingEnv.ContentRootPath, fileRoot, fileName);

                using (var stream = System.IO.File.Create(folderPath))
                {
                    await request.File.CopyToAsync(stream);
                    uploads.Add(fileRoot + "/" + request.File.FileName);
                }
            }



            return Ok(new
            {
                status = 200,
                message = "دریافت موفق",
                count = uploads.Count,
                data = uploads
            });
        }


    }
}
