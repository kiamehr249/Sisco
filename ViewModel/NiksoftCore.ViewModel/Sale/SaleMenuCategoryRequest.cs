namespace NiksoftCore.ViewModel
{
    public class SaleMenuCategoryRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImgSrc { get; set; }
        public int? ParentId { get; set; }
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
    }
}
