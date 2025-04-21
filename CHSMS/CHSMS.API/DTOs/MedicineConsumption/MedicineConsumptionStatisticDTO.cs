namespace CHSMS.API.DTOs.MedicineConsumption
{
    public class MedicineConsumptionStatisticDTO
    {
        public int MedicineConsumptionId { get; set; } 
        public int MedicineInventoryId { get; set; }
        public int? PrescriptionId { get; set; }
        public string MedicineName { get; set; } 
        public string MedicineCode { get; set; }
        public string ActiveIngredient { get; set; }
        public string Dosage { get; set; }
        public string DosageForm { get; set; }
        public double? Amount { get; set; }  
        public string? Note { get; set; } 
        public bool? Status { get; set; } 
        public string? BatchNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ConsumptionDate { get; set; }
        public DateTime? ExpiryDate { get; set; } 
        public double? TotalPrice { get; set; } 
        
        
    }
}
