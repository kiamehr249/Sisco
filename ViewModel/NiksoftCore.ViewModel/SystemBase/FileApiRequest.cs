using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ViewModel
{
    public class FileApiRequest
    {
        public IFormFile File { get; set; }
        public int FileType { get; set; }
    }
}
