using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ViewModel
{
    public class SaleMenuRequest
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long Price { get; set; }
        public long Discount { get; set; }
        public string Duration { get; set; }
        public int Inventory { get; set; }
        public int CategoryId { get; set; }
        public string ImgFile { get; set; }
        public string ImgSrc { get; set; }
        public int SaleUnitId { get; set; }
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
    }
}
