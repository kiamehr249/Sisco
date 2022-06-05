using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.ViewModel
{
    public class InvoiceRequest
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int OrderNumber { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public int? MerchantCode { get; set; }
        public long TotalFreeTaxPrice { get; set; }
        public long TotalTax { get; set; }
        public long TotalDiscount { get; set; }
        public long TotalPrice { get; set; }
        public long PaymentAmount { get; set; }
        public string Destination { get; set; }
        public int? UserId { get; set; }
        public int? DeliveryId { get; set; }
        public int OrderType { get; set; }
        public int PaymentType { get; set; }
        public int Status { get; set; }
        public bool IsPaid { get; set; }
        public int BranchId { get; set; }
        public int RestaurantId { get; set; }
        public int CashDeskId { get; set; }
        public string Description { get; set; }
        public string OrderDescription { get; set; }
        public int? PosId { get; set; }
    }
}
