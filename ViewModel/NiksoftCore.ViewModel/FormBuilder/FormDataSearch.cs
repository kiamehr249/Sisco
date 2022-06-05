using NiksoftCore.ViewModel;

namespace NiksoftCore.ViewModel
{
    public class FormDataSearch : BaseRequest
    {
        public int FormId { get; set; }
        public string DataText { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
