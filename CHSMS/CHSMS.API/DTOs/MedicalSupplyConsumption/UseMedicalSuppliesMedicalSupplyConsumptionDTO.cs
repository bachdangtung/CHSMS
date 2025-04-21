namespace CHSMS.API.DTOs.MedicalSupplyConsumption
{
    public class UseMedicalSuppliesMedicalSupplyConsumptionDTO
    {
        public int UseMedicalSupplyId { get; set; }
        public int MedicalSupplyConsumptionId { get; set; }
        public double? TotalPrice { get; set; }
    }
}