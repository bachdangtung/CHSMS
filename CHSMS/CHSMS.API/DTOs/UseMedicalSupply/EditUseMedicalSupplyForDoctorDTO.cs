namespace CHSMS.API.DTOs.UseMedicalSupply
{
    public class EditUseMedicalSupplyForDoctorDTO
    {
        public int UseMedicalSupplyId { get; set; }
        public int MedicalRecordHistoryId { get; set; }
        public int UserId { get; set; }
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public List<int> MedicalSupplyConsumptionIdsToRemove { get; set; }
        public List<CreateMedicalSupplyConsumptionDTO> MedicalSupplyConsumptionsToAdd { get; set; }
    }

}
