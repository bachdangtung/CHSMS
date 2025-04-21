namespace CHSMS.API.DTOs.Medicine
{
    public class MedicineInventoryUpdateHistoryDTO
    {
        public int MedicineInventoryId { get; set; }
        public int MedicineId { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? TransactionType { get; set; }
        public string? BatchNumber { get; set; }
        public int? SupplierId { get; set; }
        public string? Note { get; set; }
        public double? ImportQuantity { get; set; }
        public double? Quantity { get; set; }
        public DateTime? TransactionDate { get; set; }

        // Đánh dấu có thể sửa toàn bộ bản ghi không (có thể bỏ nếu không cần)
        public bool CanEdit { get; set; }

        // Các trường chỉ định có thể sửa từng trường
        public bool CanEditNote { get; set; }
        public bool CanEditImportQuantity { get; set; }
        public bool CanEditManufacturingDate { get; set; }
    }
}
