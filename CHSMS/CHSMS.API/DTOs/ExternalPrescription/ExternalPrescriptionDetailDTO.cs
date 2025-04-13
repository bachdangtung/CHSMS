using CHSMS.API.DTOs.MedicineConsumption;

namespace CHSMS.API.DTOs.ExternalPrescription
{
    public class ExternalPrescriptionDetailDTO
    {
        public int ExternalPrescriptionId { get; set; }
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
        public bool IsBhyt { get; set; }
        public List<MedicinePrescriptionDetailDTO> Medicines { get; set; } = new List<MedicinePrescriptionDetailDTO>();

    }

    public class MedicinePrescriptionDetailDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string DosageForm { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool IsBhyt { get; set; }
        
    }
}
