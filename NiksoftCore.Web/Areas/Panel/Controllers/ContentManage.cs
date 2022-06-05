using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize]
    public class ContentManage : NikController
    {
        private readonly UserManager<User> userManager;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;

        public ContentManage(
            IConfiguration config, 
            IWebHostEnvironment hostingEnv,
            UserManager<User> userManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            this.userManager = userManager;
            _hostingEnv = hostingEnv;
            _config = config;
            _iSystemBaseServ = iSystemBaseServ;
        }

        public IActionResult Index(int part)
        {
            var total = _iSystemBaseServ.iGeneralContentServ.Count(x => true);
            var pager = new Pagination(total, 10, part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "Content Management";

            ViewBag.Contents = _iSystemBaseServ.iGeneralContentServ.GetPart(x => true, pager.StartIndex, pager.PageSize).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PageTitle = "Create Content";

            var request = new ContentRequest();
            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ContentRequest request)
        {
            ViewBag.PageTitle = "Create Content";
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (!FormVlide(request))
            {
                DropDownBinder(request);
                ViewBag.Messages = Messages;
                return View(request);
            }

            string Image = string.Empty;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.ImageFile,
                    RootPath = _hostingEnv.ContentRootPath,
                    UnitPath = _config.GetSection("FileRoot:ContentDir").Value
                });

                if (!SaveImage.Success)
                {
                    DropDownBinder(request);
                    Messages.Add(new NikMessage
                    {
                        Message = "آپلود فایل انجام نشد مجدد تلاش کنید",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            var newItem = new GeneralContent
            {
                Title = request.Title,
                KeyValue = request.KeyValue,
                Header = request.Header,
                BodyText = request.BodyText,
                Footer = request.Footer,
                Icon = request.Icon,
                Image = Image,
                CategoryId = request.CategoryId,
                CreatedBy = user.Id,
                CreateDate = DateTime.Now
            };

            _iSystemBaseServ.iGeneralContentServ.Add(newItem);
            await _iSystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/ContentManage");

        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            ViewBag.PageTitle = "Create Content";

            var theItem = _iSystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            var request = new ContentRequest
            {
                Id = theItem.Id,
                Title = theItem.Title,
                KeyValue = theItem.KeyValue,
                Header = theItem.Header,
                BodyText = theItem.BodyText,
                Footer = theItem.Footer,
                Icon = theItem.Icon,
                Image = theItem.Image,
                CategoryId = theItem.CategoryId
            };
            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ContentRequest request)
        {
            ViewBag.PageTitle = "Create Content";
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (request.Id < 1)
            {
                AddError("Editing error Please perform the operation from the beginning", "fa");
            }

            if (!FormVlide(request))
            {
                DropDownBinder(request);
                ViewBag.Messages = Messages;
                return View(request);
            }

            string imageEdit = string.Empty;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var Image = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.ImageFile,
                    RootPath = _hostingEnv.ContentRootPath,
                    UnitPath = _config.GetSection("FileRoot:ContentDir").Value
                });

                if (!Image.Success)
                {
                    DropDownBinder(request);
                    Messages.Add(new NikMessage
                    {
                        Message = "File upload failed Try again",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                imageEdit = Image.FilePath;
            }



            var theContent = _iSystemBaseServ.iGeneralContentServ.Find(x => x.Id == request.Id);
            theContent.Title = request.Title;
            theContent.KeyValue = request.KeyValue;
            theContent.Header = request.Header;
            theContent.BodyText = request.BodyText;
            theContent.Footer = request.Footer;
            theContent.Icon = request.Icon;
            if (!string.IsNullOrEmpty(imageEdit))
                theContent.Image = imageEdit;
            theContent.CategoryId = request.CategoryId;
            theContent.EditedBy = user.Id;
            theContent.EditDate = DateTime.Now;
            await _iSystemBaseServ.iGeneralContentServ.SaveChangesAsync();

            return Redirect("/Panel/ContentManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = _iSystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = _hostingEnv.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            _iSystemBaseServ.iGeneralContentServ.Remove(theContent);
            await _iSystemBaseServ.iGeneralContentServ.SaveChangesAsync();
            return Redirect("/Panel/ContentManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = _iSystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            //theContent.Enabled = !theContent.Enabled;
            await _iSystemBaseServ.iGeneralContentServ.SaveChangesAsync();
            return Redirect("/Panel/ContentManage");
        }

        private void DropDownBinder(ContentRequest request)
        {
            var categories = _iSystemBaseServ.iContentCategoryServ.GetAll(x => true);
            ViewBag.Categories = new SelectList(categories, "Id", "Title", request?.CategoryId);
        }

        private bool FormVlide(ContentRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("The title must have a value", "fa");
                result = false;
            }

            return result;
        }
    }
}
