namespace CHSMS.API.DTOs.MedicalSupplyConsumption
{
    public class MedicalSupplyConsumptionStatisticDTO
    {
        public int MedicalSupplyConsumptionId { get; set; }
        public int MedicalSupplyInventoryId { get; set; }
        public int? UseMedicalSupplieId { get; set; }
        public string MedicalSupplyName { get; set; }
        public string MedicalSupplyCode { get; set; }
        public string UnitOfMeasure { get; set; }
        public double? Amount { get; set; }
        public string? Note { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ConsumptionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public double? TotalPrice { get; set; }
        
        
        
    }
}
