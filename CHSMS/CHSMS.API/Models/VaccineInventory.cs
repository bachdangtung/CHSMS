using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class VaccineInventory
    {
        public int VaccineInventoryId { get; set; }
        public int? VaccineId { get; set; }
        public bool? TransactionType { get; set; }
        public int? Quantity { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Note { get; set; }

        public virtual Vaccine? Vaccine { get; set; }
    }
}
