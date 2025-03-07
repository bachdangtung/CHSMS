using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class PrescriptionMedicineConsumption
    {
        public int MedicineConsumtionId { get; set; }
        public int PrescriptionId { get; set; }

        public virtual MedicineConsumption MedicineConsumtion { get; set; } = null!;
        public virtual Prescription Prescription { get; set; } = null!;
    }
}
