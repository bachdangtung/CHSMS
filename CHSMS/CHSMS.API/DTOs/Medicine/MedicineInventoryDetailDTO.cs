namespace CHSMS.API.DTOs.Medicine
{
    public class MedicineInventoryDetailDTO
    {
        public int MedicineInventoryId { get; set; }
        public int MedicineId { get; set; }
        public string? MedicineName { get; set; }
        public string? CertificateNumber { get; set; }
        public bool? TransactionType { get; set; }
        public double? Quantity { get; set; }
        public double? ImportQuantity { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? ReceiverId { get; set; }
        public string? ReceiverName { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Note { get; set; }
        public string? BatchNumber { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public bool IsBhyt { get; set; }
    }

}
