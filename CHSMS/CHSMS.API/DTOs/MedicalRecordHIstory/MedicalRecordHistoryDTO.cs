using CHSMS.API.DTOs.MedicalSupply;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CHSMS.API.DTOs.MedicalRecord
{
    public class MedicalRecordHistoryDTO
    {
        public int MedicalRecordHistoryId { get; set; }
        [Required(ErrorMessage = "ID bệnh án không được để trống!")]
        public int PatientId { get; set; }
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

        [Required(ErrorMessage = "Kết luận chẩn đoán không được để trống!")]
        public string? DiagnoseConclusion { get; set; }

        [Required(ErrorMessage = "Phương pháp điều trị không được để trống!")]
        public string? TreatmentMethod { get; set; }

        [Required(ErrorMessage = "Triệu chứng không được để trống!")]
        public string? Symptom { get; set; }

        public DateTime? RecordDate { get; set; }

        [Required(ErrorMessage = "Mạch không được để trống!")]
        public double? Pulse { get; set; }

        [Required(ErrorMessage = "Huyết áp không được để trống!")]
        public string? BloodPressure { get; set; }

        [Required(ErrorMessage = "Nhịp thở không được để trống!")]
        public double? RespiratoryRate { get; set; }

        [Required(ErrorMessage = "Nhiệt độ không được để trống!")]
        public double? Temperature { get; set; }

        [Required(ErrorMessage = "Chiều cao không được để trống!")]
        public double? Height { get; set; }

        [Required(ErrorMessage = "Cân nặng không được để trống!")]
        public double? Weight { get; set; }

        public string? Note { get; set; }

        [Required(ErrorMessage = "Mã bệnh án không được để trống!")]
        public string? MedicalRecordHistoryCode { get; set; }

        public double? InsuranceExemption { get; set; }

        [Required(ErrorMessage = "Đối tượng không được để trống!")]
        public string? PatientCategory { get; set; }

        [Required(ErrorMessage = "Diễn biến bệnh không được để trống!")]
        public string? DiseaseProgress { get; set; }

        public string? DiseaseStage { get; set; }
        public string? ICD { get; set; }

        [Required(ErrorMessage = "Y lệnh không được để trống!")]
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
