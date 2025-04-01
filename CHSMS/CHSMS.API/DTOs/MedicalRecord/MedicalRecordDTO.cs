using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.MedicalRecord
{
    public class MedicalRecordDTO
    {
        public int MedicalRecordId { get; set; }
        public string? PatientName { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? EthnicGroup { get; set; }
        public string? EducationLevel { get; set; }
        public string? HealthInsurance { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Job { get; set; }
        public bool? Status { get; set; }
        public string? Note { get; set; }

    }
}
