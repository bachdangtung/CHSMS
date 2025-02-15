using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class Prescription
    {
        public Prescription()
        {
            MedicalUsages = new HashSet<MedicalUsage>();
        }

        public int PrescriptionId { get; set; }
        public int? PatientId { get; set; }
        public int? UserId { get; set; }
        public double? TotalAmount { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? Diagnosis { get; set; }
        public DateTime? ReExamination { get; set; }
        public string? PaymentStatus { get; set; }
        public string? Note { get; set; }

        public virtual Patient? Patient { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<MedicalUsage> MedicalUsages { get; set; }
    }
}
