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
    public class CatContentManage : NikController
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly ISystemBaseService _iSystemBaseServ;

        public CatContentManage(
            IWebHostEnvironment hostingEnv,
            IConfiguration config,
            UserManager<User> userManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            _userManager = userManager;
            _hostingEnv = hostingEnv;
            _iSystemBaseServ = iSystemBaseServ;
        }

        public IActionResult Index(ContentGridRequest request)
        {
            var query = _iSystemBaseServ.iGeneralContentServ.ExpressionMaker();

            query.Add(x => x.CategoryId == request.CategoryId);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = _iSystemBaseServ.iGeneralContentServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "Content Manage";

            ViewBag.Contents = _iSystemBaseServ.iGeneralContentServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();
            return View(request);
        }

        [HttpGet]
        public IActionResult Create(int CatId, int Id)
        {
            ViewBag.PageTitle = "Create Content";

            var request = new ContentRequest();
            if (Id > 0)
            {
                var theItem = _iSystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
                request.Id = theItem.Id;
                request.Title = theItem.Title;
                request.KeyValue = theItem.KeyValue;
                request.Header = theItem.Header;
                request.BodyText = theItem.BodyText;
                request.Footer = theItem.Footer;
                request.Icon = theItem.Icon;
                request.Image = theItem.Image;
                request.SmallImage = theItem.SmallImage;
            }

            request.CategoryId = CatId;


            DropDownBinder(request);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContentRequest request)
        {
            ViewBag.PageTitle = "Create Content";
            var user = await _userManager.GetUserAsync(HttpContext.User);

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
                    AddError("File upload failed Try again");
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            string smallImage = string.Empty;
            if (request.SmallFile != null && request.SmallFile.Length > 0)
            {
                var SaveImage = await NikTools.SaveFileAsync(new SaveFileRequest
                {
                    File = request.SmallFile,
                    RootPath = _hostingEnv.ContentRootPath,
                    UnitPath = _config.GetSection("FileRoot:ContentDir").Value
                });

                if (!SaveImage.Success)
                {
                    DropDownBinder(request);
                    AddError("File upload failed Try again");
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                smallImage = SaveImage.FilePath;
            }

            var item = new GeneralContent();
            if (request.Id > 0)
            {
                item = await _iSystemBaseServ.iGeneralContentServ.FindAsync(x => x.Id == request.Id);
                item.EditedBy = user.Id;
                item.EditDate = DateTime.Now;
            }

            item.Title = request.Title;
            item.KeyValue = request.KeyValue;
            item.Header = request.Header;
            item.BodyText = request.BodyText;
            item.Footer = request.Footer;
            item.Icon = request.Icon;

            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;

            if (!string.IsNullOrEmpty(smallImage))
                item.SmallImage = smallImage;

            if (request.Id == 0)
            {
                item.CategoryId = request.CategoryId;
                item.CreatedBy = user.Id;
                item.CreateDate = DateTime.Now;
                _iSystemBaseServ.iGeneralContentServ.Add(item);
            }

            await _iSystemBaseServ.iContentCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/CatContentManage/?CategoryId=" + request.CategoryId);

        }


        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = _iSystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            var catId = theContent.CategoryId;
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
            return Redirect("/Panel/CatContentManage/?CategoryId=" + catId);
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = _iSystemBaseServ.iGeneralContentServ.Find(x => x.Id == Id);
            //theContent.Enabled = !theContent.Enabled;
            await _iSystemBaseServ.iGeneralContentServ.SaveChangesAsync();
            return Redirect("/Panel/CatContentManage/?CategoryId=" + theContent.CategoryId);
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
