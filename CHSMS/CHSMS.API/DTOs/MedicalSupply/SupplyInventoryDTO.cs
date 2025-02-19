namespace CHSMS.API.DTOs.MedicalSupply
{
    public class SupplyInventoryDTO
    {
        public int SupplyInventoryId { get; set; }
        public int? MedicalSupplyId { get; set; }
        public string? CertificateNumber { get; set; }
        public bool? TransactionType { get; set; }
        public double? Quantity { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ReceiverId { get; set; }
        public string? Note { get; set; }
    }
}
