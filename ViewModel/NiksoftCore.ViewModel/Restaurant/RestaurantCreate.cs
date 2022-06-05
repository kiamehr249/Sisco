namespace NiksoftCore.ViewModel
{
    public class RestaurantCreate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxBranch { get; set; }
        public string ExpirationDate { get; set; }
        public int PackageId { get; set; }
    }
}
