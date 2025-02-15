using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicalRecord
    {
        public int MedicalRecordId { get; set; }
        public int? PatientId { get; set; }
        public string? Condition { get; set; }
        public DateTime? VisitDate { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? TreatmentMethod { get; set; }
        public string? Note { get; set; }

        public virtual Patient? Patient { get; set; }
    }
}
