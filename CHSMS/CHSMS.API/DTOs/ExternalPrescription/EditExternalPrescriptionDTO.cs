namespace CHSMS.API.DTOs.ExternalPrescription
{
    public class EditExternalPrescriptionDTO
    {
        public int ExternalPrescriptionId { get; set; }
        public int MedicalRecordHistoryId { get; set; }
        public int UserId { get; set; }
        public DateTime IssueDate { get; set; }
        public string? Note { get; set; }
        public List<int> MedicinePrescriptionIdsToRemove { get; set; } = new List<int>();
        public List<MedicinePrescriptionToAddDTO> MedicinesToAdd { get; set; } = new List<MedicinePrescriptionToAddDTO>();
    }
    public class MedicinePrescriptionToAddDTO
    {
        public int MedicineId { get; set; }
        public int Amount { get; set; }
        public string? Note { get; set; }
    }
}
