using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicineConsumption
    {
        public int MedicineConsumptionId { get; set; }
        public int? MedicineInventoryId { get; set; }
        public double? Amount { get; set; }
        public DateTime? ConsumptionDate { get; set; }
        public bool? IsSpecialMedicine { get; set; }
        public string? Note { get; set; }
        public bool? Status { get; set; }

        public virtual MedicineInventory? MedicineInventory { get; set; }
    }
}
