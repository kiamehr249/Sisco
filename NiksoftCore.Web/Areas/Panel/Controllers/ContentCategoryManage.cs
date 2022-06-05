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
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize]
    public class ContentCategoryManage : NikController
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;

        public ContentCategoryManage(
            IConfiguration config, 
            IWebHostEnvironment hostingEnv,
            UserManager<User> userManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _userManager = userManager;
            _hostingEnv = hostingEnv;
            _config = config;
            _iSystemBaseServ = iSystemBaseServ;
        }

        public IActionResult Index(int part)
        {
            var query = _iSystemBaseServ.iContentCategoryServ.ExpressionMaker();
            query.Add(x => true);

            var total = _iSystemBaseServ.iContentCategoryServ.Count(query);
            var pager = new Pagination(total, 10, part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "Manage Categories";
            ViewBag.Contents = _iSystemBaseServ.iContentCategoryServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PageTitle = "Create Categories";
            var request = new ContentCategoryRequest();
            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ContentCategoryRequest request)
        {

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
                    UnitPath = _config.GetSection("FileRoot:BusinessFile").Value
                });

                if (!SaveImage.Success)
                {
                    DropDownBinder(request);
                    AddError("File upload failed Try again");
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            var newCat = new ContentCategory
            {
                Title = request.Title,
                KeyValue = request.KeyValue,
                Description = request.Description,
                Icon = request.Icon,
                Image = Image,
                ParentId = request.ParentId == 0 ? null : request.ParentId
            };

            _iSystemBaseServ.iContentCategoryServ.Add(newCat);
            await _iSystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/ContentCategoryManage");

        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            ViewBag.PageTitle = "Category Update";

            var theItem = _iSystemBaseServ.iContentCategoryServ.Find(x => x.Id == Id);
            var request = new ContentCategoryRequest
            {
                Id = theItem.Id,
                Title = theItem.Title,
                KeyValue = theItem.KeyValue,
                Description = theItem.Description,
                Icon = theItem.Icon,
                Image = theItem.Image,
                ParentId = theItem.ParentId
            };
            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ContentCategoryRequest request)
        {
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
                    UnitPath = _config.GetSection("FileRoot:BusinessFile").Value
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



            var theContent = _iSystemBaseServ.iContentCategoryServ.Find(x => x.Id == request.Id);
            theContent.Title = request.Title;
            theContent.KeyValue = request.KeyValue;
            theContent.Description = request.Description;
            theContent.Icon = request.Icon;
            if (!string.IsNullOrEmpty(imageEdit))
                theContent.Image = imageEdit;
            theContent.ParentId = request.ParentId == 0 ? null : request.ParentId;
            await _iSystemBaseServ.iContentCategoryServ.SaveChangesAsync();

            return Redirect("/Panel/ContentCategoryManage");
        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = _iSystemBaseServ.iContentCategoryServ.Find(x => x.Id == Id);
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = _hostingEnv.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            _iSystemBaseServ.iContentCategoryServ.Remove(theContent);
            await _iSystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/ContentCategoryManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = _iSystemBaseServ.iContentCategoryServ.Find(x => x.Id == Id);
            //theContent.Enabled = !theContent.Enabled;
            await _iSystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/ContentCategoryManage");
        }

        private void DropDownBinder(ContentCategoryRequest request)
        {
            var query1 = _iSystemBaseServ.iContentCategoryServ.ExpressionMaker();
            query1.Add(x => true);
            if (request.Id != 0)
            {
                query1.Add(x => x.Id != request.Id);
            }
            var categories = _iSystemBaseServ.iContentCategoryServ.GetAll(query1);
            ViewBag.Parents = new SelectList(categories, "Id", "Title", request?.ParentId);
        }

        private bool FormVlide(ContentCategoryRequest request)
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
