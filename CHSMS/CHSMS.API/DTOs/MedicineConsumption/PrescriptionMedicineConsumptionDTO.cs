namespace CHSMS.API.DTOs.MedicineConsumption
{
    public class PrescriptionMedicineConsumptionDTO
    {
        public int PrescriptionId { get; set; }
        public int MedicineConsumptionId { get; set; }
        public double? TotalPrice { get; set; }
    }
}
