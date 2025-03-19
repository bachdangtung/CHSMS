using CHSMS.API.DTOs.MedicalSupply;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CHSMS.API.DTOs.MedicalRecord
{
    public class MedicalRecordHistoryDTO
    {
        public int MedicalRecordHistoryId { get; set; }
        public int PatientId { get; set; }
        public int? UserId { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? HealthInsurance { get; set; }
        public string? Address { get; set; }
        public string? Job { get; set; }
        public string? EthnicGroup { get; set; }
        public string? UserName { get; set; }
        public string? Diagnosis { get; set; }
        public string? TreatmentMethod { get; set; }
        public string? Symptom { get; set; }
        public DateTime? RecordDate { get; set; }

        public double? Pulse { get; set; }
        public string? BloodPressure { get; set; }
        public double? RespiratoryRate { get; set; }
        public double? Temperature { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public string? Note { get; set; }
    }

    public class UserDTO
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Gender { get; set; }
    }

}
