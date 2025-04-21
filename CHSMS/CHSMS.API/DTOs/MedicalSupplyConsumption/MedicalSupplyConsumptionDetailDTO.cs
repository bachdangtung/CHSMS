namespace CHSMS.API.DTOs.MedicalSupplyConsumption
{
    public class MedicalSupplyConsumptionDetailDTO
    {
        public int MedicalSupplyConsumptionId { get; set; }
        public int MedicalSupplyId { get; set; }
        public int Amount { get; set; }
        public DateTime ConsumptionDate { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
        public string MedicalSupplyName { get; set; }
        public string BatchNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public double Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
