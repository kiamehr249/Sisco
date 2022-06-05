namespace NiksoftCore.ViewModel
{
    public class SaleStoreInputRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public double Amount { get; set; }
        public int Number { get; set; }
        public long UnitPrice { get; set; }
        public long TotalPrice { get; set; }
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
        public string Description { get; set; }
    }
}
