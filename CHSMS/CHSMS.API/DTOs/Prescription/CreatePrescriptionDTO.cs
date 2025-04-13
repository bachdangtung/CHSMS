using CHSMS.API.DTOs.MedicineConsumption;

namespace CHSMS.API.DTOs.Prescription
{
    public class CreatePrescriptionDTO
    {
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public bool IsBhyt { get; set; }
        public List<MedicineConsumptionDTO> MedicineConsumptions { get; set; }
    }
}
