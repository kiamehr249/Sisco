using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NiksoftCore.DataModel;
using NiksoftCore.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiksoftCore.Web.Widgets
{
    public class WContents : ViewComponent
    {
        private readonly IConfiguration _config;
        private readonly ISystemBaseService _iSystemBaseServ;
        public List<NikMessage> messages;

        public WContents(
            IConfiguration config,
            ISystemBaseService iSystemBaseServ
            )
        {
            _config = config;
            _iSystemBaseServ = iSystemBaseServ;
            messages = new List<NikMessage>();
        }

        public async Task<IViewComponentResult> InvokeAsync(string key, int size = 8, string viewName = "Default", int skip = 0)
        {
            var theCategory = await _iSystemBaseServ.iContentCategoryServ.FindAsync(x => x.KeyValue == key);
            List<string> KeyValues;
            IList<GeneralContent> contents;
            IList<ContentCategory> categories;
            if (theCategory != null && theCategory.Childs.Count > 0)
            {
                categories = _iSystemBaseServ.iContentCategoryServ.GetAll(x => x.Parent.KeyValue == key);
                KeyValues = categories.Select(x => x.KeyValue).ToList();
                contents = _iSystemBaseServ.iGeneralContentServ.GetPart(x => KeyValues.Contains(x.ContentCategory.KeyValue), skip, size);
                ViewBag.Categories = categories;
            }
            else
            {
                contents = _iSystemBaseServ.iGeneralContentServ.GetPart(x => x.ContentCategory.KeyValue == key, skip, size, x => x.Id , true);
            }
            ViewBag.Contents = contents;
            return View(viewName);
        }

    }
}
