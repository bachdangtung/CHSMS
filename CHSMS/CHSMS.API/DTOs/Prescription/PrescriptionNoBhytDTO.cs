namespace CHSMS.API.DTOs.Prescription
{
    public class PrescriptionNoBhytDTO
    {
        public int PrescriptionId { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public bool IsBhyt { get; set; }
        public string PatientName { get; set; }
    }
}
