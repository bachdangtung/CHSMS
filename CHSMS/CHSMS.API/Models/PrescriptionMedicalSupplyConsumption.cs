using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class PrescriptionMedicalSupplyConsumption
    {
        public int MsconsumtuionId { get; set; }
        public int PrescriptionId { get; set; }

        public virtual MedicalSupplyConsumption Msconsumtuion { get; set; } = null!;
        public virtual Prescription Prescription { get; set; } = null!;
    }
}
