namespace NiksoftCore.ViewModel
{
    public class RestaurantAdminSearch : BaseRequest
    {
        public int RestaurantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
