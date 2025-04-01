namespace CHSMS.API.DTOs.Prescription
{
    public class ListPrescriptionDTO
    {
        public int PrescriptionId { get; set; }
        public string Username { get; set; } 
        public string PatientName { get; set; } 
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
