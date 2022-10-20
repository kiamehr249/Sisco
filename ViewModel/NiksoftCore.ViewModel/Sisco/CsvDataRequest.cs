using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ViewModel
{
    public class CsvDataRequest
    {
        public IFormFile Source { get; set; }
        public string SubmitValue { get; set; }
    }
}
