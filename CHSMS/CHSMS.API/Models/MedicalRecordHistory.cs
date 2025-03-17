using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicalRecordHistory
    {
        public MedicalRecordHistory()
        {
            Prescriptions = new HashSet<Prescription>();
        }

        public int MedicalRecordHistoryId { get; set; }
        public int MedicalRecordId { get; set; }
        public DateTime? Date { get; set; }
        public string? Address { get; set; }
        public string? Diagnose { get; set; }
        public string? TreatmentMethod { get; set; }
        public double? Pulse { get; set; }
        public string? BloodPressure { get; set; }
        public double? RespiratoryRate { get; set; }
        public double? Temperature { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public int? UserId { get; set; }
        public string? Symptom { get; set; }

        public virtual MedicalRecord MedicalRecord { get; set; } = null!;
        public virtual User? User { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
    }
}
