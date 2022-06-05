using System.Collections.Generic;

namespace NiksoftCore.ViewModel
{
    public class SetOrderRequest
    {
        public int Id { get; set; }
        public int CashDeskId { get; set; }
        public int? PosId { get; set; }
        public int? UserId { get; set; }
        public int? DeliveryId { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public int? MerchantCode { get; set; }
        public string Destination { get; set; }
        public int OrderType { get; set; }
        public int PaymentType { get; set; }
        public int Status { get; set; }
        public bool IsPaid { get; set; }
        public string OrderDescription { get; set; }
        public int RestaurantId { get; set; }
        public int BranchId { get; set; }
    }
}
