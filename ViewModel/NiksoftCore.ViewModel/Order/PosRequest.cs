namespace NiksoftCore.ViewModel
{
    public class PosRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public int PosType { get; set; }
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
    }
}
