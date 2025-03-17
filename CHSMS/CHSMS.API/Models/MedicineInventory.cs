using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicineInventory
    {
        public int MedicineInventoryId { get; set; }
        public int MedicineId { get; set; }
        public string? CertificateNumber { get; set; }
        public bool? TransactionType { get; set; }
        public double? Quantity { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? ReceiverId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Note { get; set; }
        public string? BatchNumber { get; set; }
        public int? SupplierId { get; set; }

        public virtual Medicine Medicine { get; set; } = null!;
        public virtual User? Receiver { get; set; }
        public virtual Supplier? Supplier { get; set; }
    }
}
