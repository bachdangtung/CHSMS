using CHSMS.API.DTOs.MedicineConsumption;

namespace CHSMS.API.DTOs.Prescription
{
    public class EditPrescriptionForDoctorDTO
    {
        public int PrescriptionId { get; set; }
        public int MedicalRecordHistoryId { get; set; }
        public int UserId { get; set; }
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public bool IsBhyt { get; set; }
        public List<MedicineConsumptionDTO> MedicineConsumptionsToAdd { get; set; } = new List<MedicineConsumptionDTO>();
        public List<int> MedicineConsumptionIdsToRemove { get; set; } = new List<int>();
    }
}
