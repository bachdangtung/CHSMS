namespace CHSMS.API.DTOs.Prescription
{
    public class CreatePrescriptionNoBHYTDTO
    {
        public int MedicalRecordHistoryId { get; set; }
        public int UserId { get; set; }
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public bool IsBhyt { get; set; }
        public List<int> MedicineIdsToRemove { get; set; } = new List<int>();
        public List<MedicinePrescriptionNoBHYTDTO> MedicinesToAdd { get; set; }
    }
}
