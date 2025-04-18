using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class UseMedicalSuppliesMedicalSupplyConsumption
    {
        public int MsconsumptionId { get; set; }
        public int UseMedicalSupplieId { get; set; }
        public double? TotalPrice { get; set; }

        public virtual MedicalSupplyConsumption Msconsumption { get; set; } = null!;
        public virtual UseMedicalSupply UseMedicalSupplie { get; set; } = null!;
    }
}
