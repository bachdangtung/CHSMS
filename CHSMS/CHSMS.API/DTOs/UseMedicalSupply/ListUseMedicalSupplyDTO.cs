namespace CHSMS.API.DTOs.UseMedicalSupply
{
    public class ListUseMedicalSupplyDTO
    {
        public int UseMedicalSupplyId { get; set; }
        public string Username { get; set; }
        public string PatientName { get; set; }
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public decimal TotalPrice { get; set; }
    }
}