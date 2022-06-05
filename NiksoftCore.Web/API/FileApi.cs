using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.API
{
    [Route("/api/[controller]/[action]")]
    public class FileApi : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly ISystemBaseService _iSysBaseServ;

        public FileApi(
            IConfiguration config,
            IWebHostEnvironment hostingEnv,
            UserManager<User> userManager,
            ISystemBaseService iSysBaseServ
            )
        {
            _config = config;
            _hostingEnv = hostingEnv;
            _userManager = userManager;
            _iSysBaseServ = iSysBaseServ;
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromForm] FileApiRequest request)
        {
            var vExs = new string[] { ".mp4", ".mov", ".wmv", ".flv", ".avi", ".webm", ".mkv" };
            var iExs = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
            var sExs = new string[] { ".mp3", ".m4a", ".wav", ".wma" };

            var fileType = (FileType)request.FileType;

            if (request.File == null)
            {
                return BadRequest(new { message = "هیچ فایلی انتخاب نشده است." });
            }

            var fexn = Path.GetExtension(request.File.FileName).ToLower();

            switch (fileType)
            {
                case FileType.Image:
                    if (!iExs.Contains(fexn))
                        return BadRequest(new { message = "فرمت فایل انتخابی مجاز نیست." });
                    break;
                case FileType.Video:
                    if (!vExs.Contains(fexn))
                        return BadRequest(new { message = "فرمت فایل انتخابی مجاز نیست." });
                    break;
                case FileType.Sound:
                    if (!sExs.Contains(fexn))
                        return BadRequest(new { message = "فرمت فایل انتخابی مجاز نیست." });
                    break;
            }


            string fileSrc = string.Empty;
            if (request.File != null && request.File.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.File,
                    RootPath = _hostingEnv.ContentRootPath,
                    UnitPath = _config.GetSection("FileRoot:SysFiles").Value
                });

                if (!SaveImage.Success)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "بارگذاری فایل انجام نشد لطفا مجدد تلاس فرمایید."
                    });
                }

                fileSrc = SaveImage.FilePath;
            }

            return Ok(new
            {
                status = 200,
                message = "حذف موفق",
                data = fileSrc
            });
        }

    }
}
