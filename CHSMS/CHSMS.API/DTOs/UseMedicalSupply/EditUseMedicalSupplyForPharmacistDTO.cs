namespace CHSMS.API.DTOs.UseMedicalSupply
{
    public class EditUseMedicalSupplyForPharmacistDTO
    {
        public int UseMedicalSupplyId { get; set; }
        public List<MedicalSupplyConsumptionStatusDTO> MedicalSupplyConsumptionStatuses { get; set; }
    }

    public class MedicalSupplyConsumptionStatusDTO
    {
        public int MedicalSupplyConsumptionId { get; set; }
        public bool Status { get; set; }
    }
}