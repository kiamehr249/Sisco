namespace NiksoftCore.ViewModel
{
    public class CashDeskRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UnitId { get; set; }
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
    }
}
