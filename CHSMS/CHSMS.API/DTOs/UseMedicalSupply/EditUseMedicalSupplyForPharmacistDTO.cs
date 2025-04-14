namespace CHSMS.API.DTOs.UseMedicalSupply
{
    public class EditUseMedicalSupplyForPharmacistDTO
    {
        public int UseMedicalSupplyId { get; set; }
        public List<MedicalSupplyConsumptionStatusDTO> MedicalSupplyConsumptionStatuses { get; set; }
    }
}
