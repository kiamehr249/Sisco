namespace NiksoftCore.ViewModel
{
    public class SaleStoreProdRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int Number { get; set; }
        public string UnitName { get; set; }
        public long UnitPrice { get; set; }
        public int CalcType { get; set; }
        public int MinValue { get; set; }
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
    }
}
