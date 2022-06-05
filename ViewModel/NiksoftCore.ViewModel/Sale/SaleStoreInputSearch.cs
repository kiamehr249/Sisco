namespace NiksoftCore.ViewModel
{
    public class SaleStoreInputSearch : BaseRequest
    {
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
