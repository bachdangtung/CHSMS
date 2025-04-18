using CHSMS.API.DTOs.MedicalSupplyConsumption;

namespace CHSMS.API.DTOs.UseMedicalSupply
{
    public class CreateUseMedicalSupplyDTO
    {
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public List<MedicalSupplyConsumptionDTO> MedicalSupplyConsumptions { get; set; }
    }
}
