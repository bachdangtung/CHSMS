namespace CHSMS.API.DTOs.ExternalPrescription
{
    public class ExternalPrescriptionDTO
    {
        public int ExternalPrescriptionId { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public bool IsBhyt { get; set; }
        public string PatientName { get; set; }
    }
}
