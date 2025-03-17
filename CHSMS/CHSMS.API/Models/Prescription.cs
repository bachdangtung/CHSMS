using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class Prescription
    {
        public int PrescriptionId { get; set; }
        public int? MedicalRecordHistoryId { get; set; }
        public int? UserId { get; set; }
        public DateTime? IssueDate { get; set; }
        public bool? Status { get; set; }
        public string? Note { get; set; }

        public virtual MedicalRecordHistory? MedicalRecordHistory { get; set; }
        public virtual User? User { get; set; }
    }
}
