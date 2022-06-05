using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System.Threading.Tasks;

namespace NiksoftCore.FormBuilder.Controller.APIs
{
    [Route("/api/[controller]/[action]")]
    public class FormBuilderApi : NikApi
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;
        private readonly IWebHostEnvironment _hostingEnv;
        private User theUser { get; set; }

        

        public FormBuilderApi(
            IConfiguration config, 
            IWebHostEnvironment hostingEnv, 
            UserManager<User> userManager, 
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            _hostingEnv = hostingEnv;
            _userManager = userManager;
            _iSystemBaseServ = iSystemBaseServ;
        }


        [HttpPost]
        public async Task<IActionResult> FileUpload()
        {
            var file = Request.Form.Files[0];

            if (file == null || file.Length == 0)
            {
                return Ok(new
                {
                    status = 404,
                    message = "No file selected",
                    data = ""
                });
            }

            string filePath = string.Empty;
            if (file != null && file.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = file,
                    RootPath = _hostingEnv.ContentRootPath,
                    UnitPath = _config.GetSection("FileRoot:FormDir").Value
                });

                if (!SaveImage.Success)
                {
                    return Ok(new
                    {
                        status = 404,
                        message = "File upload failed Try again",
                        data = ""
                    });
                }

                filePath = SaveImage.FilePath;
            }


            return Ok(new
            {
                status = 200,
                message = "File uploaded",
                data = filePath
            });
        }





    }
}
