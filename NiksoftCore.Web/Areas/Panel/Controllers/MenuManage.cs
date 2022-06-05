﻿using Microsoft.AspNetCore.Authorization;
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize]
    public class MenuManage : NikController
    {
        private readonly UserManager<DataModel.User> _userManager;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;

        public MenuManage(
            IConfiguration config, 
            IWebHostEnvironment hostingEnv,
            UserManager<DataModel.User> userManager,
            ISystemBaseService iSystemBaseServ
            )
        {
            _userManager = userManager;
            _hostingEnv = hostingEnv;
            _config = config;
            _iSystemBaseServ = iSystemBaseServ;
        }

        public IActionResult Index(MenuCategoryGridRequest request)
        {

            var query = _iSystemBaseServ.iMenuCategoryServ.ExpressionMaker();

            query.Add(x => true);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = _iSystemBaseServ.iMenuCategoryServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "Menu Category Manage";

            ViewBag.Contents = _iSystemBaseServ.iMenuCategoryServ.GetPartOptional(query, pager.StartIndex, pager.PageSize).ToList();

            return View(request);
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "Create Menu Category";

            var request = new MenuCategoryRequest();

            if (Id > 0)
            {
                var item = _iSystemBaseServ.iMenuCategoryServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.KeyValue = item.KeyValue;
                request.Description = item.Description;
                request.Image = item.Image;
                request.Enabled = item.Enabled;
            }


            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MenuCategoryRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!CatFormValide(request))
            {
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
                    UnitPath = _config.GetSection("FileRoot:BaseSystem").Value
                });

                if (!SaveImage.Success)
                {
                    AddError("File upload failed Try again");
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            MenuCategory item;

            if (request.Id > 0)
            {
                item = _iSystemBaseServ.iMenuCategoryServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new MenuCategory();
            }

            item.Title = request.Title;
            item.KeyValue = request.KeyValue;
            item.Description = request.Description;
            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;


            if (request.Id == 0)
            {
                item.Enabled = true;
                item.CreatedBy = user.Id;
                item.CreateDate = DateTime.Now;
                _iSystemBaseServ.iMenuCategoryServ.Add(item);
            }


            await _iSystemBaseServ.iMenuCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var theContent = _iSystemBaseServ.iMenuCategoryServ.Find(x => x.Id == Id);
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = _hostingEnv.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            _iSystemBaseServ.iMenuCategoryServ.Remove(theContent);
            await _iSystemBaseServ.iMenuCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var theContent = _iSystemBaseServ.iMenuCategoryServ.Find(x => x.Id == Id);
            theContent.Enabled = !theContent.Enabled;
            await _iSystemBaseServ.iMenuCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage");
        }

        private void DropDownBinder(ContentRequest request)
        {
            var categories = _iSystemBaseServ.iContentCategoryServ.GetAll(x => true);
            ViewBag.Categories = new SelectList(categories, "Id", "Title", request?.CategoryId);
        }

        private bool CatFormValide(MenuCategoryRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("The title must have a value");
                result = false;
            }

            return result;
        }


        public IActionResult MenuGrid(MenuGridRequest request)
        {
            var category = _iSystemBaseServ.iMenuCategoryServ.Find(x => x.Id == request.CategoryId);
            ViewBag.Category = category;

            Menu parent = new Menu();
            if (request.ParentId > 0)
            {
                parent = _iSystemBaseServ.iMenuServ.Find(x => x.Id == request.ParentId);
            }

            ViewBag.Parent = parent;

            var query = _iSystemBaseServ.iMenuServ.ExpressionMaker();

            query.Add(x => x.CategoryId == request.CategoryId);

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query.Add(x => x.Title.Contains(request.Title));
                isSearch = true;
            }
            ViewBag.Search = isSearch;

            if (request.ParentId != 0)
                query.Add(x => x.ParentId == request.ParentId);
            else
                query.Add(x => x.ParentId == null);

            var total = _iSystemBaseServ.iMenuServ.Count(query);
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.PageTitle = "Menu Manage / " + category.Title;

            ViewBag.Contents = _iSystemBaseServ.iMenuServ.GetPart(query, pager.StartIndex, pager.PageSize, x => x.OrderId, true).ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenu(int Id, int ParentId, int CategoryId)
        {

            var category = await _iSystemBaseServ.iMenuCategoryServ.FindAsync(x => x.Id == CategoryId);

            Menu parent = new Menu();
            if (ParentId > 0)
            {
                parent = await _iSystemBaseServ.iMenuServ.FindAsync(x => x.Id == ParentId);
            }

            ViewBag.PageTitle = category.Title + " / Create Items / " + parent.Title;

            var request = new MenuRequest();

            if (Id > 0)
            {
                var item = _iSystemBaseServ.iMenuServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.Link = item.Link;
                request.Description = item.Description;
                request.Image = item.Image;
                request.Enabled = item.Enabled;
                request.OrderId = item.OrderId;
                request.CategoryId = item.CategoryId;
                request.ParentId = item.ParentId != null ? item.ParentId.Value : 0;
            }
            else
            {
                request.CategoryId = CategoryId;
                request.ParentId = ParentId;
            }


            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromForm] MenuRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!MenuFormValide(request))
            {
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
                    UnitPath = _config.GetSection("FileRoot:BaseSystem").Value
                });

                if (!SaveImage.Success)
                {
                    Messages.Add(new NikMessage
                    {
                        Message = "File upload failed Try again",
                        Type = MessageType.Error,
                        Language = "Fa"
                    });
                    ViewBag.Messages = Messages;
                    return View(request);
                }

                Image = SaveImage.FilePath;
            }

            Menu item;

            if (request.Id > 0)
            {
                item = _iSystemBaseServ.iMenuServ.Find(x => x.Id == request.Id);
                item.EditDate = DateTime.Now;
                item.EditedBy = user.Id;
            }
            else
            {
                item = new Menu();
            }

            item.Title = request.Title;
            item.Link = request.Link;
            item.Description = request.Description;
            if (!string.IsNullOrEmpty(Image))
                item.Image = Image;

            item.CategoryId = request.CategoryId;
            item.ParentId = request.ParentId == 0 ? null : request.ParentId;


            if (request.Id == 0)
            {
                var query = _iSystemBaseServ.iMenuServ.ExpressionMaker();
                query.Add(x => x.CategoryId == request.CategoryId);

                if (request.ParentId > 0)
                    query.Add(x => x.ParentId == request.ParentId);
                else
                    query.Add(x => x.ParentId == null);

                var maxOrder = _iSystemBaseServ.iMenuServ.Count(query);
                item.OrderId = maxOrder + 1;
                item.Enabled = true;
                item.CreatedBy = user.Id;
                item.CreateDate = DateTime.Now;
                _iSystemBaseServ.iMenuServ.Add(item);
            }

            int? parentId = null;
            if (request.ParentId > 0)
            {
                parentId = request.ParentId;
            }


            await _iSystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + request.CategoryId + "&ParentId=" + parentId);

        }

        public async Task<IActionResult> RemoveMenu(int Id)
        {
            var theContent = _iSystemBaseServ.iMenuServ.Find(x => x.Id == Id);
            var catId = theContent.CategoryId;
            var parentId = theContent.ParentId;
            if (!string.IsNullOrEmpty(theContent.Image))
            {
                NikTools.RemoveFile(new RemoveFileRequest
                {
                    RootPath = _hostingEnv.ContentRootPath,
                    FilePath = theContent.Image
                });
            }

            _iSystemBaseServ.iMenuServ.Remove(theContent);
            await _iSystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + catId + "&ParentId=" + parentId);
        }

        public async Task<IActionResult> EnableMenu(int Id)
        {
            var theContent = _iSystemBaseServ.iMenuServ.Find(x => x.Id == Id);
            theContent.Enabled = !theContent.Enabled;
            await _iSystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theContent.CategoryId + "&ParentId=" + theContent.ParentId);
        }

        public async Task<IActionResult> OrderUpMenu(int Id)
        {
            var theItem = _iSystemBaseServ.iMenuServ.Find(x => x.Id == Id);
            var itemsCount = _iSystemBaseServ.iMenuServ.Count(x => x.CategoryId == theItem.CategoryId && x.ParentId == theItem.ParentId);
            if (theItem.OrderId == itemsCount)
            {
                return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theItem.CategoryId + "&ParentId=" + theItem.ParentId);
            }

            var upItem = _iSystemBaseServ.iMenuServ.Find(x => x.CategoryId == theItem.CategoryId && x.ParentId == theItem.ParentId && x.OrderId == (theItem.OrderId + 1));

            theItem.OrderId = theItem.OrderId + 1;
            upItem.OrderId = upItem.OrderId - 1;

            await _iSystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theItem.CategoryId + "&ParentId=" + theItem.ParentId);
        }

        public async Task<IActionResult> OrderDownMenu(int Id)
        {
            var theItem = _iSystemBaseServ.iMenuServ.Find(x => x.Id == Id);
            if (theItem.OrderId == 1)
            {
                return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theItem.CategoryId + "&ParentId=" + theItem.ParentId);
            }

            var upItem = _iSystemBaseServ.iMenuServ.Find(x => x.CategoryId == theItem.CategoryId && x.ParentId == theItem.ParentId && x.OrderId == (theItem.OrderId - 1));

            theItem.OrderId = theItem.OrderId - 1;
            upItem.OrderId = upItem.OrderId + 1;

            await _iSystemBaseServ.iMenuServ.SaveChangesAsync();
            return Redirect("/Panel/MenuManage/MenuGrid?CategoryId=" + theItem.CategoryId + "&ParentId=" + theItem.ParentId);
        }

        private bool MenuFormValide(MenuRequest request)
        {
            bool result = true;
            if (string.IsNullOrEmpty(request.Title))
            {
                AddError("The title must have a value");
                result = false;
            }

            if (request.CategoryId == 0)
            {
                AddError("The category must have a value");
                result = false;
            }

            return result;
        }


    }
}
