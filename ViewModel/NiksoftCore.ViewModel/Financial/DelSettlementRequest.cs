namespace NiksoftCore.ViewModel
{
    public class DelSettlementRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public long InvoicesAmount { get; set; }
        public long PosAmount { get; set; }
        public long RemainedAmount { get; set; }
        public int PosId { get; set; }
        public int UserId { get; set; }
        public int ToInvoiceId { get; set; }
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
        public string Description { get; set; }
    }
}
