using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ViewModel
{
    public class BranchCreate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Tels { get; set; }
        public string Supervisor { get; set; }
        public IFormFile ImgFile { get; set; }
        public string ImgSrc { get; set; }
        public IFormFile LogoFile { get; set; }
        public string Logo { get; set; }
        public int RestaurantId { get; set; }
        public int CityId { get; set; }
    }
}
