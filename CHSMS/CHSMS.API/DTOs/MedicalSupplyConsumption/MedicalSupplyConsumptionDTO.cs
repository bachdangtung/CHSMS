namespace CHSMS.API.DTOs.MedicalSupplyConsumption
{
    public class MedicalSupplyConsumptionDTO
    {
        public int MedicalSupplyInventoryId { get; set; }
        public double Amount { get; set; }
        public DateTime ConsumptionDate { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
    }
}
