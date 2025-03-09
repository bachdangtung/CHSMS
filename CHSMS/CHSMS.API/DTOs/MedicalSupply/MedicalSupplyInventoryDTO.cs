using CHSMS.API.Models;

namespace CHSMS.API.DTOs.MedicalSupply
{
    public class MedicalSupplyInventoryDTO
    {
        public int SupplyInventoryId { get; set; }
        public int? MedicalSupplyId { get; set; }
        public string? CertificateNumber { get; set; }
        public bool? TransactionType { get; set; }
        public double? Quantity { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? ReceiverId { get; set; }
        public string? Note { get; set; }
        //public virtual MedicalSupplyConsumption? MedicalSupplyConsumption { get; set; }
    }
}
