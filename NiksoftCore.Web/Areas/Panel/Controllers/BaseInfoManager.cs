using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NiksoftCore.DataModel;
using NiksoftCore.MiddlController.Middles;
using NiksoftCore.Utilities;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize]
    public class BaseInfoManager : NikController
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;

        public BaseInfoManager(
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

        [HttpGet]
        public async Task<IActionResult> Index(BaseInfoSearch request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.PageTitle = "اطلاعات پایه";

            var myPrefix = await _iSystemBaseServ.iBaseInfoServ.FindAsync(x => x.KeyValue == "PreFix" && x.UserId == user.Id);
            
            if(myPrefix == null)
            {
                myPrefix = new BaseInfo
                {
                    Title = "پیش شماره",
                    Description = "مقداری که دستگاه بصورت پیشفرض اضافه می کند",
                    KeyValue = "PreFix",
                    GroupValue = "Sisco",
                    StringValue = null,
                    ViewObject = "[{\"FieldType\": 5, \"Title\": \"مقدار پیش فرض دستگاه\"}, {\"FieldType\": 7, \"Title\": \"برای مقادیر خروجی هم اضافه میشود؟\"}]",
                    UserId = user.Id,
                    CreateDate = DateTime.Now,
                };
                _iSystemBaseServ.iBaseInfoServ.Add(myPrefix);
                await _iSystemBaseServ.iBaseInfoServ.SaveChangesAsync();
            }

            var query = _iSystemBaseServ.iBaseInfoServ.QueryMaker(y => y.Where(x => x.UserId == user.Id));

            bool isSearch = false;
            if (!string.IsNullOrEmpty(request.Title))
            {
                query = query.Where(x => x.Title.Contains(request.Title));
                isSearch = true;
            }

            ViewBag.Search = isSearch;

            var total = query.Count();
            var pager = new Pagination(total, 10, request.part);
            ViewBag.Pager = pager;

            ViewBag.Contents = query.OrderByDescending(x => x.Id).Skip(pager.StartIndex).Take(pager.PageSize).ToList();

            return View(request);
        }

        [HttpGet]
        public IActionResult Create(int Id)
        {

            ViewBag.PageTitle = "Create Menu Category";

            var request = new BaseInfoRequest();

            if (Id > 0)
            {
                var item = _iSystemBaseServ.iBaseInfoServ.Find(x => x.Id == Id);
                request.Id = item.Id;
                request.Title = item.Title;
                request.KeyValue = item.KeyValue;
                request.GroupValue = item.GroupValue;
                request.Description = item.Description;
                request.MinValue = item.MinValue;
                request.MaxValue = item.MaxValue;
                request.IntValue = item.IntValue;
                request.DoubleValue = item.DoubleValue;
                request.StringValue = item.StringValue;
                request.LongValue = item.LongValue;
                request.BoolValue = item.BoolValue;
                if (item.StartDate != null)
                    request.StartDate = item.StartDate.Value.ToPersianDateString();
                if (item.EndDate != null)
                    request.EndDate = item.EndDate.Value.ToPersianDateString();
                request.ViewObject = item.ViewObject;
                request.Enabled = item.Enabled;
                request.ViewModels = JsonConvert.DeserializeObject<List<BaseInfoViewModel>>(item.ViewObject);
            }


            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BaseInfoRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!FormValide(request))
            {
                ViewBag.Messages = Messages;
                return View(request);
            }

            var item = new BaseInfo();

            if (request.Id > 0)
            {
                item = _iSystemBaseServ.iBaseInfoServ.Find(x => x.Id == request.Id);
                item.ModifyDate = DateTime.Now;
            }

            item.Title = request.Title;
            item.KeyValue = request.KeyValue;
            item.GroupValue = request.GroupValue;
            item.Description = request.Description;
            item.MinValue = request.MinValue;
            item.MaxValue = request.MaxValue;
            item.IntValue = request.IntValue;
            item.DoubleValue = request.DoubleValue;
            item.StringValue = request.StringValue;
            item.LongValue = request.LongValue;
            item.BoolValue = request.BoolValue;

            if (!string.IsNullOrEmpty(request.StartDate))
                item.StartDate = PersianDateTime.Parse(request.StartDate).ToDateTime();

            if (!string.IsNullOrEmpty(request.EndDate))
                item.EndDate = PersianDateTime.Parse(request.EndDate).ToDateTime();

            item.ViewObject = request.ViewObject;

            if (request.Id == 0)
            {
                item.Enabled = true;
                item.UserId = user.Id;
                item.CreateDate = DateTime.Now;
                _iSystemBaseServ.iBaseInfoServ.Add(item);
            }

            await _iSystemBaseServ.iMenuCategoryServ.SaveChangesAsync();
            return Redirect("/Panel/BaseInfoManager");

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var item = _iSystemBaseServ.iBaseInfoServ.Find(x => x.Id == Id);

            _iSystemBaseServ.iBaseInfoServ.Remove(item);
            await _iSystemBaseServ.iBaseInfoServ.SaveChangesAsync();
            return Redirect("/Panel/BaseInfoManager");
        }

        public async Task<IActionResult> Enable(int Id)
        {
            var item = _iSystemBaseServ.iBaseInfoServ.Find(x => x.Id == Id);
            item.Enabled = !item.Enabled;
            await _iSystemBaseServ.iBaseInfoServ.SaveChangesAsync();
            return Redirect("/Panel/BaseInfoManager");
        }

        private bool FormValide(BaseInfoRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
                AddError("عنوان باید مقدار داشته باشد.");

            if (string.IsNullOrEmpty(request.KeyValue))
                AddError("کلید باید مقدار داشته باشد.");

            if (string.IsNullOrEmpty(request.GroupValue))
                AddError("گروه باید مقدار داشته باشد.");

            if (Messages.Count(x => x.Type == MessageType.Error) > 0)
                return false;

            return true;
        }

    }
}
