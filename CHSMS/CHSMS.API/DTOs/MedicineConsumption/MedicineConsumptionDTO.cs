namespace CHSMS.API.DTOs.MedicineConsumption
{
    public class MedicineConsumptionDTO
    {
        public int MedicineInventoryId { get; set; }
        public double Amount { get; set; }
        public DateTime ConsumptionDate { get; set; }
        public string Note { get; set; }
        public bool IsSpecialMedicine { get; set; } 
        public bool Status { get; set; }
    }
}
