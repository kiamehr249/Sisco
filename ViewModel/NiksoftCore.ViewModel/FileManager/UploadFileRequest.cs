using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ViewModel
{
    public class UploadFileRequest
    {
        public IFormFile File { get; set; }
        public string ContentId { get; set; }
        public string ContentKey { get; set; }
        public string RootKey { get; set; }
    }
}
