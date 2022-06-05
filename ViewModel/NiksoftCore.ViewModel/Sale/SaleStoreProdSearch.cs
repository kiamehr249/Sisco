namespace NiksoftCore.ViewModel
{
    public class SaleStoreProdSearch : BaseRequest
    {
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
        public string Title { get; set; }
    }
}
