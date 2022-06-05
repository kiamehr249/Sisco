namespace NiksoftCore.ViewModel
{
    public class SaleMenuCategorySearch : BaseRequest
    {
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
    }
}
