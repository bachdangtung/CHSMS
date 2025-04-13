using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicinePrescription
    {
        public int MedicineId { get; set; }
        public int ExternalPrescriptionId { get; set; }
        public string? Note { get; set; }
        public int? Amount { get; set; }

        public virtual ExternalPrescription ExternalPrescription { get; set; } = null!;
        public virtual Medicine Medicine { get; set; } = null!;
    }
}
