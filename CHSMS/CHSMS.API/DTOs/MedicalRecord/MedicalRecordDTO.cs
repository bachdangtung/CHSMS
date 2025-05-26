using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.MedicalRecord
{
    public class MedicalRecordDTO
    {
        public int MedicalRecordId { get; set; }

        [Required(ErrorMessage = "Tên bệnh nhân không được để trống!")]
        public string? PatientName { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống!")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống!")]
        public DateTime? Dob { get; set; }

        [Required(ErrorMessage = "Dân tộc không được để trống!")]
        public string? EthnicGroup { get; set; }

        [Required(ErrorMessage = "Trình độ học vấn không được để trống!")]
        public string? EducationLevel { get; set; }

        public string? HealthInsurance { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống!")]
        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        [Required(ErrorMessage = "Nghề nghiệp không được để trống!")]
        public string? Job { get; set; }

        public bool? Status { get; set; }
        public string? Note { get; set; }
        public DateTime? DateCreated { get; set; }

    }
}
