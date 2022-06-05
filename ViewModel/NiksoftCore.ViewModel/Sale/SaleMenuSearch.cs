namespace NiksoftCore.ViewModel
{
    public class SaleMenuSearch : BaseRequest
    {
        public int CategoryId { get; set; }
        public int SaleUnitId { get; set; }
        public string Title { get; set; }
    }
}
