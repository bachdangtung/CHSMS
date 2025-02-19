using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class SupplyInventory
    {
        public int SupplyInventoryId { get; set; }
        public int? MedicalSupplyId { get; set; }
        public string? CertificateNumber { get; set; }
        public bool? TransactionType { get; set; }
        public double? Quantity { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ReceiverId { get; set; }
        public string? Note { get; set; }

        public virtual MedicalSupply? MedicalSupply { get; set; }
        public virtual User? Receiver { get; set; }
    }
}
