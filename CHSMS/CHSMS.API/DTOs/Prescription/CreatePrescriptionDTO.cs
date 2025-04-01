using CHSMS.API.DTOs.MedicineConsumption;

namespace CHSMS.API.DTOs.Prescription
{
    public class CreatePrescriptionDTO
    {
        public int MedicalRecordHistoryId { get; set; }
        public int UserId { get; set; }
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public bool IsBhyt { get; set; }
        public List<MedicineConsumptionDTO> MedicineConsumptions { get; set; }
    }
}
