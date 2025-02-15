using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicalInventory
    {
        public int MedicalInventoryId { get; set; }
        public int? MedicineId { get; set; }
        public string? DecisionLetter { get; set; }
        public bool? TransactionType { get; set; }
        public int? Quantity { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Note { get; set; }

        public virtual Medicine? Medicine { get; set; }
    }
}
