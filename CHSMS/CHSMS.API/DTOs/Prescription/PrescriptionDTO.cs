using CHSMS.API.Models;

namespace CHSMS.API.DTOs
{
    public class PrescriptionDTO
    {
        public int PrescriptionId { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public bool IsBhyt { get; set; }
        public string PatientName { get; set; }
    }
    
}
