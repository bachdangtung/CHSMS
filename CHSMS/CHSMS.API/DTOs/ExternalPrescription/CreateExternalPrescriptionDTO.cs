namespace CHSMS.API.DTOs.ExternalPrescription
{
    public class CreateExternalPrescriptionDTO
    {
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public bool IsBhyt { get; set; }
        public List<MedicinePrescriptionDTO> MedicinesToAdd { get; set; }
    }
}
