using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicalSupplyConsumption
    {
        public int MsconsumptionId { get; set; }
        public int? Msid { get; set; }
        public double? Amount { get; set; }
        public DateTime? ConsumptionDate { get; set; }
        public bool? Bhyt { get; set; }
        public string? Note { get; set; }

        public virtual MedicalSupplyInventory Msconsumption { get; set; } = null!;
    }
}
