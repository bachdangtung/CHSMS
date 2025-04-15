using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicalSupplyConsumption
    {
        public int MsconsumptionId { get; set; }
        public int MedicalSupplyInventoryId { get; set; }
        public double? Amount { get; set; }
        public DateTime? ConsumptionDate { get; set; }
        public string? Note { get; set; }
        public bool? Status { get; set; }

        public virtual MedicalSupplyInventory MedicalSupplyInventory { get; set; } = null!;
    }
}
