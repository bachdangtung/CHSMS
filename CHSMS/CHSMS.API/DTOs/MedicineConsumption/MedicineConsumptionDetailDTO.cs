namespace CHSMS.API.DTOs.MedicineConsumption
{
    public class MedicineConsumptionDetailDTO
    {
        public int MedicineConsumptionId { get; set; }
        public int Amount { get; set; }
        public DateTime ConsumptionDate { get; set; }
        public string Note { get; set; }
        public bool IsSpecialMedicine { get; set; }
        public bool Status { get; set; }
        public string MedicineName { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public double? Quantity { get; set; }
        public string DosageForm {  get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsBhyt { get; set; }
    }
}
