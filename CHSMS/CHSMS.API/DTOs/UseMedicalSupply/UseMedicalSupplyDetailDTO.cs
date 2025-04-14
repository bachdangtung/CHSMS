namespace CHSMS.API.DTOs.UseMedicalSupply
{
    public class UseMedicalSupplyDetailDTO
    {
        public int UseMedicalSupplyId { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public string FullName { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public DateTime Dob { get; set; }
        public string Address { get; set; }
        public string HealthInsurance { get; set; }
        public string DiagnoseConclusion { get; set; }
        public List<MedicalSupplyConsumptionDetailDTO> MedicalSupplyConsumptions { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
