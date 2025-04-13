using CHSMS.API.DTOs.Validations;
using System.ComponentModel.DataAnnotations;

namespace CHSMS.API.DTOs.Medicine
{
    public class MedicineInventoryAddDTO
    {
        [Required(ErrorMessage = "Thuốc là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID thuốc phải lớn hơn 0.")]
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "Số chứng nhận là bắt buộc.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Số chứng nhận phải từ 1 đến 100 ký tự.")]
        public string CertificateNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại giao dịch là bắt buộc.")]
        public bool TransactionType { get; set; }

        [Required(ErrorMessage = "Số lượng nhập là bắt buộc.")]
        [Range(1, double.MaxValue, ErrorMessage = "Số lượng nhập phải lớn hơn 0.")]
        public double ImportQuantity { get; set; }

        [Required(ErrorMessage = "Ngày sản xuất là bắt buộc.")]
        [DataType(DataType.Date)]
        [CustomDateValidation(ErrorMessage = "Ngày sản xuất phải nhỏ hơn hoặc bằng ngày hiện tại.")]
        public DateTime ManufacturingDate { get; set; }

        [Required(ErrorMessage = "Ngày nhập là bắt buộc.")]
        [DataType(DataType.Date)]
        [CustomDateValidation(ErrorMessage = "Ngày nhập phải nhỏ hơn hoặc bằng ngày hiện tại.")]
        public DateTime TransactionDate { get; set; }

        [StringLength(250, ErrorMessage = "Ghi chú không được vượt quá 250 ký tự.")]
        public string Note { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số lô là bắt buộc.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Số lô phải từ 1 đến 50 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9-]+$", ErrorMessage = "Số lô chỉ được chứa chữ cái, số và dấu gạch ngang.")]
        public string BatchNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nhà cung cấp là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID nhà cung cấp phải lớn hơn 0.")]
        public int SupplierId { get; set; }
    }

}
