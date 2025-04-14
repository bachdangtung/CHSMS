namespace CHSMS.API.DTOs.UseMedicalSupply
{
    public class UseMedicalSupplyDTO
    {
        public int UseMedicalSupplyId { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public string PatientName { get; set; }
    }
}
