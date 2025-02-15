using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class Patient
    {
        public Patient()
        {
            Appointments = new HashSet<Appointment>();
            MedicalRecords = new HashSet<MedicalRecord>();
            MedicalUsages = new HashSet<MedicalUsage>();
            Prescriptions = new HashSet<Prescription>();
            TransferedPatients = new HashSet<TransferedPatient>();
            VaccinationRecords = new HashSet<VaccinationRecord>();
        }

        public int PatientId { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? EthnicGroup { get; set; }
        public string? EducationalLevel { get; set; }
        public string? HealthInsurance { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
        public virtual ICollection<MedicalUsage> MedicalUsages { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<TransferedPatient> TransferedPatients { get; set; }
        public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; }
    }
}
