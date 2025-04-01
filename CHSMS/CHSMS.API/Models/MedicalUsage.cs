using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicalUsage
    {
        public int UsageId { get; set; }
        public int? MedicineId { get; set; }
        public int? PatientId { get; set; }
        public int? PrescriptionId { get; set; }
        public int? QuantityRefunded { get; set; }
        public DateTime? TransferedDate { get; set; }
        public string? ReturnDate { get; set; }
        public string? Status { get; set; }

        public virtual Medicine? Medicine { get; set; }
        public virtual Prescription? Prescription { get; set; }
    }
}
