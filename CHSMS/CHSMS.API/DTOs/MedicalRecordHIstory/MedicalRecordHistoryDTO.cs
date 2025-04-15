using CHSMS.API.DTOs.MedicalSupply;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CHSMS.API.DTOs.MedicalRecord
{
    public class MedicalRecordHistoryDTO
    {
        public int MedicalRecordHistoryId { get; set; }
        [Required(ErrorMessage = "Tên bệnh nhân không được để trống.")]
        public int PatientId { get; set; }
        //[Required(ErrorMessage = "Bác sĩ phụ trách không được để trống.")]
        public int? UserId { get; set; }
        public string? DoctorName { get; set; }
        public string? Fullname { get; set; }
        public string? PatientName { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? HealthInsurance { get; set; }
        public string? Address { get; set; }
        public string? Job { get; set; }
        public string? EthnicGroup { get; set; }
        public string? UserName { get; set; }
        public string? DiagnoseConclusion { get; set; }
        public string? TreatmentMethod { get; set; }
        public string? Symptom { get; set; }
        //[Required(ErrorMessage = "Ngày tạo không được để trống.")]
        public DateTime? RecordDate { get; set; }

        public double? Pulse { get; set; }
        public string? BloodPressure { get; set; }
        public double? RespiratoryRate { get; set; }
        public double? Temperature { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public string? Note { get; set; }
        public string? MedicalRecordHistoryCode { get; set; }
        public double? InsuranceExemption { get; set; }
        public string? PatientCategory { get; set; }
        public string? DiseaseProgress { get; set; }
        public string? DiseaseStage { get; set; }
        public string? ICD { get; set; }
        public string? MedicalOrder { get; set; }
        public string? TreatmentBed { get; set; }
    }

    public class UserDTO
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Gender { get; set; }
    }

}
