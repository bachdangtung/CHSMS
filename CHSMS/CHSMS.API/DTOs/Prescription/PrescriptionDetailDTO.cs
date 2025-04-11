using CHSMS.API.DTOs.MedicineConsumption;

namespace CHSMS.API.DTOs.Prescription
{
    public class PrescriptionDetailDTO
    {
        public int PrescriptionId { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public string UserName { get; set; }
        public string PatientName { get; set; }
        public string HealthInsurance { get; set; }
        public string DiagnoseConclusion { get; set; }
        public List<MedicineConsumptionDetailDTO> MedicineConsumptions { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
