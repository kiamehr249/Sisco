namespace NiksoftCore.ViewModel
{
    public class AddressRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public int UserId { get; set; }
    }
}
